﻿// ---------------------------------------------------------------------------------------
// Copyright (C) Riccardo De Agostini and contributors. All rights reserved.
// Licensed under the MIT license.
// See the LICENSE file in the project root for full license information.
//
// Part of this file may be third-party code, distributed under a compatible license.
// See the THIRD-PARTY-NOTICES file in the project root for third-party copyright notices.
// ---------------------------------------------------------------------------------------

using Buildvana.Sdk.CodeGeneration.Configuration;
using NUnit.Framework;

namespace Buildvana.Sdk.CodeGeneration.Tests;

public class CodeGeneratorTest
{
    [TestCase("C#")]
    [TestCase("VisualBasic")]
    public void GenerateCode_ReturnsNonEmptyString(string languageStr)
    {
        var assemblyAttribute = new AssemblyAttributeFragment(
            "System.Runtime.InteropServices.ComVisible",
            new[] { new OrderedParameter(false) });

        var thisAssemblyClass = new ThisAssemblyClassFragment(
            null,
            "ThisAssembly",
            new[] { new Constant("Answer", 42) });

        _ = CodeGeneratorConfigurationUtility.TryParseLanguage(languageStr, out var language);
        var configuration = new GeneratedCode(
            language,
            new CodeFragment[] { assemblyAttribute, thisAssemblyClass });

        var code = CodeGenerator.GenerateCode(configuration);
        Assert.IsNotNull(code);
        Assert.IsNotEmpty(code);
    }
}
