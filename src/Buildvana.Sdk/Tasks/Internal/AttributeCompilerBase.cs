// -----------------------------------------------------------------------------------
// Copyright (C) Riccardo De Agostini and Buildvana contributors. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
//
// Part of this file may be third-party code, distributed under a compatible license.
// See THIRD-PARTY-NOTICES file in the project root for third-party copyright notices.
// -----------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Buildvana.Sdk.Internal;
using Microsoft.Build.Framework;

using ModuleStrings = Buildvana.Sdk.Tasks.Internal.Strings.LiteralAssemblyAttributes;

namespace Buildvana.Sdk.Tasks.Internal
{
    internal abstract class AttributeCompilerBase : IAttributeCompiler
    {
        public abstract string Extension { get; }

        public void AppendAttribute(StringBuilder sb, ITaskItem attributeItem)
        {
            // Some attributes only allow positional constructor arguments, or the user may just prefer them.
            // To set those, use metadata names like "_Parameter1", "_Parameter2" etc.
            // If a parameter index is skipped, it's an error.
            var customMetadata = attributeItem.CloneCustomMetadata();

            var orderedParameters = new List<string>(customMetadata.Count + 1 /* max possible slots needed */);
            var namedParameters = new Dictionary<string, string>();

            foreach (DictionaryEntry entry in customMetadata)
            {
                var name = (string)entry.Key;
                var value = (string)entry.Value;

                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new BuildErrorException(ModuleStrings.EmptyParameterFmt, name);
                }

                value = TransformParameterValue(value.Trim());

                if (name.StartsWith("_Parameter", StringComparison.OrdinalIgnoreCase))
                {
                    if (!int.TryParse(name.Substring("_Parameter".Length), out var index))
                    {
                        throw new BuildErrorException(ModuleStrings.InvalidParameterNameFmt, name);
                    }

                    if (index < 1)
                    {
                        throw new BuildErrorException(ModuleStrings.InvalidParameterIndexFmt, name);
                    }

                    if (index > orderedParameters.Count + 1)
                    {
                        throw new BuildErrorException(ModuleStrings.SkippedNumberedParameterFmt, index);
                    }

                    // "_Parameter01" and "_Parameter1" would overwrite each other.
                    if (index > orderedParameters.Count)
                    {
                        orderedParameters.Add(value);
                    }
                    else
                    {
                        orderedParameters[index - 1] = value;
                    }
                }
                else
                {
                    namedParameters.Add(name, value);
                }
            }

            var encounteredNull = false;
            for (var i = 0; i < orderedParameters.Count; i++)
            {
                if (orderedParameters[i] == null)
                {
                    encounteredNull = true;
                }
                else if (encounteredNull)
                {
                    throw new BuildErrorException(ModuleStrings.SkippedNumberedParameterFmt, i + 1);
                }
            }

            AppendAttributeCore(sb, attributeItem.ItemSpec, orderedParameters, namedParameters);
        }

        protected abstract void AppendAttributeCore(StringBuilder sb, string attributeType, IReadOnlyList<string> orderedParameters, IReadOnlyDictionary<string, string> namedParameters);

        protected abstract string TransformParameterValue(string value);
    }
}