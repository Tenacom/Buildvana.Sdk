// -----------------------------------------------------------------------------------
// Copyright (C) Riccardo De Agostini and Buildvana contributors. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
//
// Part of this file may be third-party code, distributed under a compatible license.
// See THIRD-PARTY-NOTICES file in the project root for third-party copyright notices.
// -----------------------------------------------------------------------------------

#if NETFRAMEWORK

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Buildvana.Sdk.Utilities;

namespace Buildvana.Sdk.Tasks.Internal
{
#pragma warning disable CA1812 // Avoid uninstantiated internal classes - Instantiated via Install static method
    internal sealed class AssemblyResolver : MarshalByRefObject, IDisposable
#pragma warning restore CA1812
    {
        private AssemblyResolver(string[] directories)
        {
            Directories = directories;
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        private IEnumerable<string> Directories { get; }

        public static AssemblyResolver Install(AppDomain appDomain, params string[] directories)
            => (AssemblyResolver)appDomain.CreateInstanceAndUnwrap(
                Assembly.GetExecutingAssembly().FullName,
                typeof(AssemblyResolver).FullName,
                false,
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new object[] { directories },
                null,
                null);

        public static AssemblyResolver Install(AppDomain appDomain, IEnumerable<string> directories)
            => Install(appDomain, directories.ToArray());

        public void Dispose() => AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;

        private Assembly? CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var assemblyName = new AssemblyName(args.Name);
            var simpleName = assemblyName.Name;
            var dllPaths = Directories
                .Select(directory => Path.Combine(directory, simpleName + ".dll"))
                .Where(File.Exists);
            foreach (var dllPath in dllPaths)
            {
                try
                {
                    return Assembly.LoadFile(dllPath);
                }
                catch (Exception e) when (!e.IsCriticalException())
                {
                    // Nothing to do
                }
            }

            return null;
        }
    }
}

#endif
