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
using System.Linq;
using System.Reflection;
using System.Threading;
using Buildvana.Sdk.Tasks.Internal;
using Buildvana.Sdk.Utilities;
using JetBrains.Annotations;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Buildvana.Sdk.Tasks
{
    // Copyright (C) Andrew Arnott and Nerdbank.MSBuildExtension contributors.
    // Licensed under the Microsoft Public License.
    // This file has been modified from the original form.
    // Original file: https://github.com/AArnott/Nerdbank.MSBuildExtension/blob/master/src/Nerdbank.MSBuildExtension/net45/ContextIsolatedTask.cs
    // See THIRD-PARTY-NOTICES in the repository root for more information.
    public abstract partial class ContextIsolatedTask : AppDomainIsolatedTask // We need MarshalByRefObject -- we don't care for MSBuild's AppDomain though.
    {
        private readonly ThreadLocal<bool> _resolvingAssembly = new();

        public sealed override bool Execute()
        {
            // We have to hook our own AppDomain so that the TransparentProxy works properly.
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            try
            {
                // On .NET Framework (on Windows), we find native binaries by adding them to our PATH.
                if (UnmanagedDllDirectory != null)
                {
                    var pathEnvVar = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
                    var searchPaths = pathEnvVar.Split(new[] { Path.PathSeparator }, StringSplitOptions.RemoveEmptyEntries);
                    if (!searchPaths.Contains(UnmanagedDllDirectory, StringComparer.OrdinalIgnoreCase))
                    {
                        pathEnvVar += Path.PathSeparator + UnmanagedDllDirectory;
                        Environment.SetEnvironmentVariable("PATH", pathEnvVar);
                    }
                }

                // Run under our own AppDomain so we can apply the .config file of the MSBuild Task we're hosting.
                // This gives the owner the control over binding redirects to be applied.
                var pathToTaskAssembly = GetType().Assembly.Location;
                var appDomainSetup = new AppDomainSetup
                {
                    ApplicationBase = Path.GetDirectoryName(pathToTaskAssembly),
                    ConfigurationFile = pathToTaskAssembly + ".config",
                };
                var appDomain = AppDomain.CreateDomain("ContextIsolatedTask: " + GetType().Name, AppDomain.CurrentDomain.Evidence, appDomainSetup);

                using var resolver = AssemblyResolver.Install(
                    appDomain,
                    "Microsoft.Build",
                    Path.GetDirectoryName(new Uri(typeof(AppDomainIsolatedTask).Assembly.Location).LocalPath)!);

                var taskType = GetType();
                var assemblyName = taskType.Assembly.FullName;
                var typeName = taskType.FullName!;
                var innerTask = (ContextIsolatedTask?)Activator.CreateInstance(assemblyName, typeName)?.Unwrap();
                if (innerTask == null)
                {
                    throw new TypeLoadException($"Cannot create an instance of '{typeName}' from '{assemblyName}'.");
                }

                return ExecuteInnerTask(innerTask);
            }
            catch (OperationCanceledException)
            {
                Log.LogMessage(MessageImportance.High, $"Task {GetType().Name} canceled.");
                return false;
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
            }
        }

#pragma warning disable CA1822 // Mark members as static - This is for consistency with .NET Core code
        [PublicAPI]
        protected Assembly LoadAssemblyByPath(string assemblyPath) => Assembly.LoadFile(assemblyPath);
#pragma warning restore CA1822

        private Assembly? CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (_resolvingAssembly.Value)
            {
                // Guard against stack overflow exceptions.
                return null;
            }

            _resolvingAssembly.Value = true;
            try
            {
                return Assembly.Load(args.Name);
            }
            catch (Exception e) when (!e.IsCriticalException())
            {
                return null;
            }
            finally
            {
                _resolvingAssembly.Value = false;
            }
        }
    }
}

#endif
