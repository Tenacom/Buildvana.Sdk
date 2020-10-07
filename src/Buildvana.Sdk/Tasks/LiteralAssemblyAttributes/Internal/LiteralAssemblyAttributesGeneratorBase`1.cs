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
using Buildvana.Sdk.Tasks.Internal;

namespace Buildvana.Sdk.Tasks.LiteralAssemblyAttributes.Internal
{
    internal abstract class LiteralAssemblyAttributesGeneratorBase<TBuilder> : CodeGenerator<TBuilder>, ILiteralAssemblyAttributesGenerator
        where TBuilder : CodeBuilder, new()
    {
        public string GenerateCode(IEnumerable<AttributeDefinition> attributes)
        {
            foreach (var attribute in attributes)
            {
                var orderedParameters = attribute.OrderedParameters.Select(TransformParameterValue).ToArray();
                var namedParameters = attribute.NamedParameters.ToDictionary(
                    pair => pair.Key,
                    pair => TransformParameterValue(pair.Value));

                GenerateAttribute(attribute.Type, orderedParameters, namedParameters);
            }

            return GetGeneratedCode();
        }

        protected abstract string TransformParameterValue(string value);

        protected abstract void GenerateAttribute(string type, IReadOnlyCollection<string> orderedParameters, IReadOnlyDictionary<string, string> namedParameters);
    }
}