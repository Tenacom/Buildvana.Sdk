// -----------------------------------------------------------------------------------
// Copyright (C) Riccardo De Agostini and Buildvana contributors. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
//
// Part of this file may be third-party code, distributed under a compatible license.
// See THIRD-PARTY-NOTICES file in the project root for third-party copyright notices.
// -----------------------------------------------------------------------------------

#if !NETFRAMEWORK
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Buildvana.Sdk.Tasks
{
    // Copyright (C) Andrew Arnott and Nerdbank.MSBuildExtension contributors.
    // Licensed under the Microsoft Public License.
    // This file has been modified from the original form.
    // Original file: https://github.com/AArnott/Nerdbank.MSBuildExtension/blob/master/src/Nerdbank.MSBuildExtension/netstandard1.5/ContextIsolatedTask.cs
    // See THIRD-PARTY-NOTICES in the repository root for more information.
    public abstract partial class ContextIsolatedTask : Task
    {
        private AssemblyLoadContext? _context;

        public sealed override bool Execute()
        {
            try
            {
                var taskAssemblyPath = new Uri(GetType().Assembly.Location).LocalPath;
                _context = new CustomAssemblyLoader(this);
                var inContextAssembly = _context.LoadFromAssemblyPath(taskAssemblyPath);
                var innerTaskType = inContextAssembly.GetType(GetType().FullName!);

                var innerTask = Activator.CreateInstance(innerTaskType!);
                return ExecuteInnerTask(innerTask!);
            }
            catch (OperationCanceledException)
            {
                Log.LogMessage(MessageImportance.High, $"Task {GetType().Name} canceled.");
                return false;
            }
        }

        protected Assembly LoadAssemblyByPath(string assemblyPath)
        {
            if (_context == null)
            {
                throw new InvalidOperationException($"{nameof(AssemblyLoadContext)} must be set before calling {nameof(LoadAssemblyByPath)}.");
            }

            return _context.LoadFromAssemblyPath(assemblyPath);
        }

        private class CustomAssemblyLoader : AssemblyLoadContext
        {
            private readonly ContextIsolatedTask _loaderTask;

            internal CustomAssemblyLoader(ContextIsolatedTask loaderTask)
            {
                _loaderTask = loaderTask;
            }

            protected override Assembly Load(AssemblyName assemblyName)
                => _loaderTask.LoadAssemblyByName(assemblyName) ?? Default.LoadFromAssemblyName(assemblyName);

            protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
            {
                string? unmanagedDllPath = null;
                var unmanagedDllDirectory = _loaderTask.UnmanagedDllDirectory;
                if (unmanagedDllDirectory != null)
                {
                    unmanagedDllPath = Directory.EnumerateFiles(
                            unmanagedDllDirectory,
                            $"{unmanagedDllName}.*")
                        .Concat(Directory.EnumerateFiles(
                            unmanagedDllDirectory,
                            $"lib{unmanagedDllName}.*"))
                        .FirstOrDefault();
                }

                return unmanagedDllPath != null
                    ? LoadUnmanagedDllFromPath(unmanagedDllPath)
                    : base.LoadUnmanagedDll(unmanagedDllName);
            }
        }
    }
}

#endif