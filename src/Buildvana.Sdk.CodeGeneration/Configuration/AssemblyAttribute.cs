// ---------------------------------------------------------------------------------------
// Copyright (C) Riccardo De Agostini and contributors. All rights reserved.
// Licensed under the MIT license.
// See the LICENSE file in the project root for full license information.
//
// Part of this file may be third-party code, distributed under a compatible license.
// See the THIRD-PARTY-NOTICES file in the project root for third-party copyright notices.
// ---------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace Buildvana.Sdk.CodeGeneration.Configuration;
#pragma warning disable CA1711 // Rename type so that it does not end in 'Attribute' - Does anyone have a better name for this type?
public sealed class AssemblyAttribute : CodeFragment
#pragma warning restore CA1711
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
