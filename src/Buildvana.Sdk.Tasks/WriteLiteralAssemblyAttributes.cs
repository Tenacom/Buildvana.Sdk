// ---------------------------------------------------------------------------------------
// Copyright (C) Riccardo De Agostini and contributors. All rights reserved.
// Licensed under the MIT license.
// See the LICENSE file in the project root for full license information.
//
// Part of this file may be third-party code, distributed under a compatible license.
// See the THIRD-PARTY-NOTICES file in the project root for third-party copyright notices.
// ---------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Buildvana.Sdk.CodeGeneration.Configuration;
using Buildvana.Sdk.Tasks.Base;
using Buildvana.Sdk.Tasks.Resources;
using JetBrains.Annotations;
using Microsoft.Build.Framework;

namespace Buildvana.Sdk.Tasks;

public sealed class WriteLiteralAssemblyAttributes : BuildvanaSdkCodeGeneratorTask
{
    [PublicAPI]
#pragma warning disable CA1819 // Properties should not return arrays - Required by MSBuild APIs
    public ITaskItem[]? LiteralAssemblyAttributes { get; set; }
#pragma warning restore CA1819

    protected override IEnumerable<CodeFragment> GetCodeFragments()
        => (LiteralAssemblyAttributes ?? Enumerable.Empty<ITaskItem>()).Select(ExtractAttributeFromItem);

    private static AssemblyAttributeFragment ExtractAttributeFromItem(ITaskItem item)
    {
        var type = item.ItemSpec;

        // Some attributes only allow positional constructor arguments, or the user may just prefer them.
        // To set those, use metadata names like "_Parameter1", "_Parameter2" etc.
        // If a parameter index is skipped, it's an error.
        var customMetadata = item.CloneCustomMetadata() ?? new Dictionary<string, string>();

        var orderedParameters = new List<object?>(customMetadata.Count + 1 /* max possible slots needed */);
        var namedParameters = new Dictionary<string, object?>();

        foreach (var customMetadataEntry in customMetadata)
        {
            if (customMetadataEntry is not DictionaryEntry entry)
            {
                continue;
            }

            var name = (string)entry.Key;
            var valueStr = (string?)entry.Value;

            if (!CodeGeneratorConfigurationUtility.TryParseConstantValue(valueStr, out var value))
            {
                throw new BuildErrorException(Strings.LiteralAssemblyAttributes.InvalidParameterValueFmt, name);
            }

            if (name.StartsWith("_Parameter", StringComparison.OrdinalIgnoreCase))
            {
                if (!int.TryParse(name["_Parameter".Length..], out var index))
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

        return new AssemblyAttributeFragment(
            type,
            orderedParameters.Select(p => new OrderedParameter(p)),
            namedParameters.Select(p => new NamedParameter(p.Key, p.Value)));
    }
}
