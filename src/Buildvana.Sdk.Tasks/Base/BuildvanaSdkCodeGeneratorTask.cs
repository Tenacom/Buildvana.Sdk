// -----------------------------------------------------------------------------------
// Copyright (C) Riccardo De Agostini and Buildvana contributors. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
//
// Part of this file may be third-party code, distributed under a compatible license.
// See THIRD-PARTY-NOTICES file in the project root for third-party copyright notices.
// -----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Text;
using Buildvana.Sdk.CodeGeneration;
using Buildvana.Sdk.CodeGeneration.Configuration;
using Buildvana.Sdk.Tasks.Resources;
using Buildvana.Sdk.Utilities;
using JetBrains.Annotations;
using Microsoft.Build.Framework;

namespace Buildvana.Sdk.Tasks.Base
{
    public abstract class BuildvanaSdkCodeGeneratorTask : BuildvanaSdkIsolatedTask
    {
        [Required]
        [PublicAPI]
        public string? Language { get; set; }

        [Required]
        public string? OutputPath { get; set; }

        protected sealed override void Run()
        {
            if (!CodeGeneratorConfigurationUtility.TryParseLanguage(Language, out var codeGeneratorLanguage))
            {
                throw new BuildErrorException(Strings.UnsupportedLanguageFmt, Language ?? "<null>");
            }

            if (StringUtility.IsNullOrEmpty(OutputPath))
            {
                throw new BuildErrorException(Strings.MissingParameterFmt, nameof(OutputPath));
            }

            var generatedCode = new GeneratedCode(codeGeneratorLanguage, GetCodeFragments());
            var code = CodeGenerator.GenerateCode(generatedCode);
            File.WriteAllText(OutputPath, code, Encoding.UTF8);
        }

        protected abstract IEnumerable<CodeFragment> GetCodeFragments();
    }
}
