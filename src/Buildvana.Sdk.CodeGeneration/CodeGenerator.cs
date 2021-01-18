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
using System.Diagnostics.CodeAnalysis;
using Buildvana.Sdk.CodeGeneration.Configuration;
using Buildvana.Sdk.CodeGeneration.Internal;

namespace Buildvana.Sdk.CodeGeneration
{
    public abstract class CodeGenerator
    {
        private static readonly IReadOnlyDictionary<CodeGeneratorLanguage, Func<CodeGenerator>> Factories
            = new Dictionary<CodeGeneratorLanguage, Func<CodeGenerator>>
            {
                { CodeGeneratorLanguage.CSharp, () => new CSharpCodeGenerator() },
                { CodeGeneratorLanguage.VisualBasic, () => new VisualBasicCodeGenerator() },
            };

        public static string GenerateCode(GeneratedCode configuration)
        {
            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (!TryCreate(configuration.Language, out var generator))
            {
                throw new Exception($"Unknown language {configuration.Language} for code generator.");
            }

            return generator.GenerateCodeCore(configuration.CodeFragments);
        }

        protected abstract string GenerateCodeCore(IEnumerable<CodeFragment> codeFragments);

        private static bool TryCreate(CodeGeneratorLanguage language, [MaybeNullWhen(false)] out CodeGenerator result)
        {
            if (!Factories.TryGetValue(language, out var factory))
            {
                result = null;
                return false;
            }

            result = factory();
            return true;
        }
    }
}
