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
using System.Linq;

namespace Buildvana.Sdk.CodeGeneration.Configuration
{
    public sealed class AssemblyAttribute : CodeFragment
    {
        public AssemblyAttribute(
            string type,
            IEnumerable<OrderedParameter> orderedParameters,
            IEnumerable<NamedParameter>? namedParameters = null)
        {
            Type = type;
            OrderedParameters = orderedParameters.ToArray();
            NamedParameters = namedParameters?.ToArray() ?? Array.Empty<NamedParameter>();
        }

        public string Type { get; }

        public IReadOnlyList<OrderedParameter> OrderedParameters { get; }

        public IReadOnlyList<NamedParameter> NamedParameters { get; }
    }
}
