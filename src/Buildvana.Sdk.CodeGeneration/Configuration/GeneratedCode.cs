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

public sealed class GeneratedCode
{
    public GeneratedCode(CodeGeneratorLanguage language, IEnumerable<CodeFragment> codeFragments)
    {
        Language = language;
        CodeFragments = codeFragments.ToArray();
    }

    public CodeGeneratorLanguage Language { get; }

    public IReadOnlyList<CodeFragment> CodeFragments { get; }
}
