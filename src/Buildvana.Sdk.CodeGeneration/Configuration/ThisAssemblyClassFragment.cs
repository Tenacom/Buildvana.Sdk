// ---------------------------------------------------------------------------------------
// Copyright (C) Riccardo De Agostini and contributors. All rights reserved.
// Licensed under the MIT license.
// See the LICENSE file in the project root for full license information.
//
// Part of this file may be third-party code, distributed under a compatible license.
// See the THIRD-PARTY-NOTICES file in the project root for third-party copyright notices.
// ---------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;

namespace Buildvana.Sdk.CodeGeneration.Configuration;

public sealed class ThisAssemblyClassFragment : CodeFragment
{
    public ThisAssemblyClassFragment(
        string? @namespace,
        string name,
        IEnumerable<Constant> constants)
    {
        Namespace = @namespace;
        Name = name;
        Constants = constants.ToArray();
    }

    public string? Namespace { get; }

    public string Name { get; }

    public IReadOnlyList<Constant> Constants { get; }
}
