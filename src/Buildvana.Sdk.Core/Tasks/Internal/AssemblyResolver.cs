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
using System.IO;
using System.Reflection;
using Buildvana.Sdk.Utilities;

namespace Buildvana.Sdk.Tasks.Internal
{
#pragma warning disable CA1812 // Avoid uninstantiated internal classes - Instantiated via Install static method
    internal sealed class AssemblyResolver : MarshalByRefObject, IDisposable
#pragma warning restore CA1812
    {
        public AssemblyResolver(string prefix, string directory)
        {
            Prefix = prefix;
            Directory = directory;
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        private string Prefix { get; }

        private string Directory { get; }

        public static IDisposable Install(AppDomain appDomain, string prefix, string directory)
            => (AssemblyResolver)appDomain.CreateInstanceAndUnwrap(
                Assembly.GetExecutingAssembly().FullName,
                typeof(AssemblyResolver).FullName,
                false,
                BindingFlags.Default,
                null,
                new object[] { prefix, directory },
                null,
                null);

        public void Dispose() => AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;

        private Assembly? CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (!args.Name.StartsWith(Prefix, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            try
            {
                var assemblyName = new AssemblyName(args.Name);
                var simpleName = assemblyName.Name;
                var dllPath = Path.Combine(Directory, simpleName + ".dll");
                if (File.Exists(dllPath))
                {
                    return Assembly.LoadFile(dllPath);
                }
            }
            catch (Exception e) when (!e.IsCriticalException())
            {
            }

            return null;
        }
    }
}

#endif
