// -----------------------------------------------------------------------------------
// Copyright (C) Riccardo De Agostini and Buildvana contributors. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
//
// Part of this file may be third-party code, distributed under a compatible license.
// See THIRD-PARTY-NOTICES file in the project root for third-party copyright notices.
// -----------------------------------------------------------------------------------

using System.Collections.Generic;

namespace Buildvana.Sdk.Tasks.Internal
{
    internal partial class VisualBasicCodeGenerator
    {
        protected override void GenerateAttribute(AttributeModifier modifier, string type, IReadOnlyCollection<string> orderedParameters, IReadOnlyDictionary<string, string> namedParameters)
        {
            Text('<');
            Text(GetAttributeModifierPrefix(modifier));
            Text(type);

            if (orderedParameters.Count > 0 || namedParameters.Count > 0)
            {
                Text('(');
                var first = true;

                foreach (var parameter in orderedParameters)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        Text(", ");
                    }

                    Text(parameter);
                }

                foreach (var pair in namedParameters)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        Text(", ");
                    }

                    Text(pair.Key);
                    Text(":=");
                    Text(pair.Value);
                }

                Text(')');
            }

            Text('>');
        }

        private static string GetAttributeModifierPrefix(AttributeModifier target)
            => target switch {
                AttributeModifier.Assembly => "Assembly:",
                AttributeModifier.Module => "Module:",
                _ => string.Empty,
            };
    }
}