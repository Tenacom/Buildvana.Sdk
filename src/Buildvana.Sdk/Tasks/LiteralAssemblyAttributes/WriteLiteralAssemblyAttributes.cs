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
using System.IO;
using System.Linq;
using System.Text;
using Buildvana.Sdk.Tasks.Internal;
using Buildvana.Sdk.Tasks.LiteralAssemblyAttributes.Internal;
using Buildvana.Sdk.Tasks.Resources;
using JetBrains.Annotations;
using Microsoft.Build.Framework;

namespace Buildvana.Sdk.Tasks.LiteralAssemblyAttributes
{
    public sealed class WriteLiteralAssemblyAttributes : BuildvanaSdkTask
    {
        [PublicAPI]
        [Required]
        public string? Language { get; set; }

        [PublicAPI]
#pragma warning disable CA1819 // Properties should not return arrays - Required by MSBuild API
        public ITaskItem[]? LiteralAssemblyAttributes { get; set; }
#pragma warning restore CA1819

        [PublicAPI]
        [Required]
        public string? OutputPath { get; set; }

        protected override void Run()
        {
            Language ??= string.Empty;
            if (!LiteralAssemblyAttributesGenerator.TryCreate(Language, out var generator))
            {
                throw new BuildErrorException(Strings.UnsupportedLanguageFmt, Language);
            }

            if (StringUtility.IsNullOrEmpty(OutputPath))
            {
                throw new BuildErrorException(Strings.MissingParameterFmt, nameof(OutputPath));
            }

            var attributes = ExtractAttributesFromItems(LiteralAssemblyAttributes ?? Enumerable.Empty<ITaskItem>());
            var code = generator.GenerateCode(attributes);

            try
            {
                // Overwrites file if it already exists (and can be overwritten)
                File.WriteAllText(OutputPath, code, Encoding.UTF8);
            }
            catch (Exception e) when (e.IsIORelatedException())
            {
                throw new BuildErrorException(Strings.CouldNotWriteFileFmt, OutputPath, e.Message);
            }
        }

        private IEnumerable<(string Type, IReadOnlyCollection<string> OrderedParameters, IReadOnlyDictionary<string, string> NamedParameters)> ExtractAttributesFromItems(IEnumerable<ITaskItem> items)
            => items.Select(ExtractAttributeFromItem);

        private (string Type, IReadOnlyCollection<string> OrderedParameters, IReadOnlyDictionary<string, string> NamedParameters) ExtractAttributeFromItem(ITaskItem item)
        {
            var type = item.ItemSpec;

            // Some attributes only allow positional constructor arguments, or the user may just prefer them.
            // To set those, use metadata names like "_Parameter1", "_Parameter2" etc.
            // If a parameter index is skipped, it's an error.
            var customMetadata = item.CloneCustomMetadata();

            var orderedParameters = new List<string>(customMetadata.Count + 1 /* max possible slots needed */);
            var namedParameters = new Dictionary<string, string>();

            foreach (DictionaryEntry entry in customMetadata)
            {
                var name = (string)entry.Key;
                var value = (string)entry.Value;

                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new BuildErrorException(Strings.LiteralAssemblyAttributes.EmptyParameterFmt, name);
                }

                if (name.StartsWith("_Parameter", StringComparison.OrdinalIgnoreCase))
                {
                    if (!int.TryParse(name.Substring("_Parameter".Length), out var index))
                    {
                        throw new BuildErrorException(Strings.LiteralAssemblyAttributes.InvalidParameterNameFmt, name);
                    }

                    if (index < 1)
                    {
                        throw new BuildErrorException(Strings.LiteralAssemblyAttributes.InvalidParameterIndexFmt, name);
                    }

                    if (index > orderedParameters.Count + 1)
                    {
                        throw new BuildErrorException(Strings.LiteralAssemblyAttributes.SkippedNumberedParameterFmt, index);
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
                    throw new BuildErrorException(Strings.LiteralAssemblyAttributes.SkippedNumberedParameterFmt, i + 1);
                }
            }

            return (type, orderedParameters, namedParameters);
        }
    }
}