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
using JetBrains.Annotations;

namespace Buildvana.Sdk.CodeGeneration.Configuration
{
    [PublicAPI]
    public static class GeneratedCodeExtensions
    {
        public static GeneratedCode WithCodeFragment(this GeneratedCode @this, CodeFragment fragment)
#pragma warning disable SA1000 // new should be followed by a space - StyleCop doesn't understand new C# 9 syntax
            => new(@this.Language, @this.CodeFragments.Append(fragment));
#pragma warning restore SA1000

        public static GeneratedCode WithCodeFragments(this GeneratedCode @this, IEnumerable<CodeFragment> fragments)
#pragma warning disable SA1000 // new should be followed by a space - StyleCop doesn't understand new C# 9 syntax
            => new(@this.Language, @this.CodeFragments.Concat(fragments));
#pragma warning restore SA1000
    }
}
