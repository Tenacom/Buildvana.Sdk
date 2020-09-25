// -----------------------------------------------------------------------------------
// Copyright (C) Riccardo De Agostini and Buildvana contributors. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
//
// Part of this file may be third-party code, distributed under a compatible license.
// See THIRD-PARTY-NOTICES file in the project root for third-party copyright notices.
// -----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Text;

namespace Buildvana.Sdk.Tasks.Internal
{
    internal sealed class VisualBasicAttributeCompiler : AttributeCompilerBase
    {
        public override string Extension => ".vb";

        protected override void AppendAttributeCore(StringBuilder sb, string attributeType, IReadOnlyList<string> orderedParameters, IReadOnlyDictionary<string, string> namedParameters)
        {
            _ = sb.Append("<Assembly: ").Append(attributeType);

            if (orderedParameters.Count > 0 || namedParameters.Count > 0)
            {
                _ = sb.Append('(');
                var first = true;

                foreach (var parameter in orderedParameters)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        _ = sb.Append(", ");
                    }

                    _ = sb.Append(parameter);
                }

                foreach (var pair in namedParameters)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        _ = sb.Append(", ");
                    }

                    _ = sb.Append(pair.Key).Append(":=").Append(pair.Value);
                }

                _ = sb.Append(')');
            }

            _ = sb.Append('>').AppendLine();
        }

        protected override string TransformParameterValue(string value)
            => value.ToUpperInvariant() switch {
                "TRUE" => "True",
                "FALSE" => "False",
                "NULL" => "Nothing",
                _ => value,
            };
    }
}