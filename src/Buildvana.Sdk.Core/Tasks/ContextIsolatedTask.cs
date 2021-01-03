// -----------------------------------------------------------------------------------
// Copyright (C) Riccardo De Agostini and Buildvana contributors. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
//
// Part of this file may be third-party code, distributed under a compatible license.
// See THIRD-PARTY-NOTICES file in the project root for third-party copyright notices.
// -----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using JetBrains.Annotations;
using Microsoft.Build.Framework;

namespace Buildvana.Sdk.Tasks
{
    // Copyright (C) Andrew Arnott and Nerdbank.MSBuildExtension contributors.
    // Licensed under the Microsoft Public License.
    // This file has been modified from the original form.
    // Original file: https://github.com/AArnott/Nerdbank.MSBuildExtension/blob/master/src/Nerdbank.MSBuildExtension/ContextIsolatedTask.cs
    // See THIRD-PARTY-NOTICES in the repository root for more information.
    [PublicAPI]
    public abstract partial class ContextIsolatedTask : ICancelableTask
    {
        private readonly CancellationTokenSource _cts = new();

        protected virtual string ManagedDllDirectory => Path.GetDirectoryName(new Uri(GetType().Assembly.Location).LocalPath)!;

        protected virtual string? UnmanagedDllDirectory => null;

        [PublicAPI]
        protected CancellationToken CancellationToken => _cts.Token;

        public void Cancel() => _cts.Cancel();

        protected abstract bool ExecuteIsolated();

        protected virtual Assembly? LoadAssemblyByName(AssemblyName assemblyName)
        {
            if (assemblyName == null)
            {
                throw new ArgumentNullException(nameof(assemblyName));
            }

            var assemblySimpleName = assemblyName.Name ?? string.Empty;
            if (assemblySimpleName.StartsWith("Microsoft.Build", StringComparison.OrdinalIgnoreCase)
             || assemblySimpleName.StartsWith("System.", StringComparison.OrdinalIgnoreCase))
            {
                // MSBuild and System.* make up our exchange types. So don't load them in this LoadContext.
                // We need to inherit them from the default load context.
                return null;
            }

            var assemblyPath = Path.Combine(ManagedDllDirectory, assemblySimpleName) + ".dll";
            return File.Exists(assemblyPath) ? LoadAssemblyByPath(assemblyPath) : null;
        }

        private bool ExecuteInnerTask(object innerTask)
        {
            if (innerTask == null)
            {
                throw new ArgumentNullException(nameof(innerTask));
            }

            var outerType = GetType();
            var innerType = innerTask.GetType();

            static Dictionary<string, PropertyInfo> GetPropertyDictionary(Type type)
                => type.GetRuntimeProperties()
                    .Where(i => i.GetMethod != null && i.SetMethod != null)
                    .ToDictionary(i => i.Name);

            var outerProperties = GetPropertyDictionary(outerType);
            var innerProperties = GetPropertyDictionary(innerType);
            var propertiesDiscovery
                = from outerProperty in outerProperties.Values
                  let innerProperty = innerProperties[outerProperty.Name]
                  select new { outerProperty, innerProperty };

            var propertiesMap = propertiesDiscovery.ToArray();
            var outputPropertiesMap = propertiesMap
                .Where(pair => pair.outerProperty.GetCustomAttribute<OutputAttribute>() != null)
                .ToArray();

            foreach (var propertyPair in propertiesMap)
            {
                propertyPair.innerProperty.SetValue(innerTask, propertyPair.outerProperty.GetValue(this));
            }

            // Forward any cancellation requests
            var innerCancelMethod = innerType.GetMethod(nameof(Cancel))!;
            using (CancellationToken.Register(() => innerCancelMethod.Invoke(innerTask, Array.Empty<object>())))
            {
                CancellationToken.ThrowIfCancellationRequested();

                // Execute the inner task.
                var executeInnerMethod = innerType.GetMethod(nameof(ExecuteIsolated), BindingFlags.Instance | BindingFlags.NonPublic)!;
                var result = (bool)executeInnerMethod.Invoke(innerTask, Array.Empty<object>())!;

                // Retrieve any output properties.
                foreach (var propertyPair in outputPropertiesMap)
                {
                    propertyPair.outerProperty.SetValue(this, propertyPair.innerProperty.GetValue(innerTask));
                }

                return result;
            }
        }
    }
}