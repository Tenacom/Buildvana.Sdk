// -----------------------------------------------------------------------------------
// Copyright (C) Riccardo De Agostini and Buildvana contributors. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
//
// Part of this file may be third-party code, distributed under a compatible license.
// See THIRD-PARTY-NOTICES file in the project root for third-party copyright notices.
// -----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;

namespace Buildvana.Sdk.Tasks.Internal
{
    internal partial class CodeGenerator
    {
        public void GenerateLiteralAssemblyAttributes(IEnumerable<AttributeDefinition> attributes)
        {
            foreach (var attribute in attributes)
            {
                NewLine();
                GenerateAttribute(
                    AttributeModifier.Assembly,
                    attribute.Type,
                    attribute.OrderedParameters.Select(TransformLiteralConstant).ToArray(),
                    attribute.NamedParameters.ToDictionary(
                        pair => pair.Key,
                        pair => TransformLiteralConstant(pair.Value)));
            }
        }

        protected abstract string TransformLiteralConstant(string value);
    }
}