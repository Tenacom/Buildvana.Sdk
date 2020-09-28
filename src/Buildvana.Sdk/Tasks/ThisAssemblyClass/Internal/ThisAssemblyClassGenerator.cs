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

namespace Buildvana.Sdk.Tasks.ThisAssemblyClass.Internal
{
    internal static class ThisAssemblyClassGenerator
    {
        private static readonly IReadOnlyDictionary<string, Func<IThisAssemblyClassGenerator>> Factories
            = new Dictionary<string, Func<IThisAssemblyClassGenerator>>(StringComparer.OrdinalIgnoreCase)
            {
                { "C#", () => new CSharpThisAssemblyClassGenerator() },
                { "VB", () => new VisualBasicThisAssemblyClassGenerator() },
            };

        public static bool TryCreate(string language, [MaybeNullWhen(false)] out IThisAssemblyClassGenerator result)
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