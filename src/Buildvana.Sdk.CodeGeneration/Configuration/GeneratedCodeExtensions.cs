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
using JetBrains.Annotations;

namespace Buildvana.Sdk.CodeGeneration.Configuration;

[PublicAPI]
public static class GeneratedCodeExtensions
{
    public static GeneratedCode WithCodeFragment(this GeneratedCode @this, CodeFragment fragment)
        => new(@this.Language, @this.CodeFragments.Append(fragment));

    public static GeneratedCode WithCodeFragments(this GeneratedCode @this, IEnumerable<CodeFragment> fragments)
        => new(@this.Language, @this.CodeFragments.Concat(fragments));
}
