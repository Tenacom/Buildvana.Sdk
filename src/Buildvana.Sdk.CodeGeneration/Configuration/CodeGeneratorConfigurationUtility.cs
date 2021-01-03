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
using System.Globalization;
using System.Linq;
using Buildvana.Sdk.Utilities;

namespace Buildvana.Sdk.CodeGeneration.Configuration
{
    public static class CodeGeneratorConfigurationUtility
    {
        private static readonly IReadOnlyDictionary<string, CodeGeneratorLanguage> AllowedLanguages
            = new Dictionary<string, CodeGeneratorLanguage>
            {
                { "CSharp", CodeGeneratorLanguage.CSharp },
                { "cs", CodeGeneratorLanguage.CSharp },
                { "C#", CodeGeneratorLanguage.CSharp },
                { "VisualBasic", CodeGeneratorLanguage.VisualBasic },
                { "VB", CodeGeneratorLanguage.VisualBasic },
            };

        private static readonly IReadOnlyDictionary<string, Type> AllowedConstantTypes
            = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                { typeof(byte).FullName!, typeof(byte) },
                { "byte", typeof(byte) },
                { "uint8", typeof(byte) },
                { typeof(short).FullName!, typeof(short) },
                { "short", typeof(short) },
                { "int16", typeof(short) },
                { typeof(int).FullName!, typeof(int) },
                { "int", typeof(int) },
                { "int32", typeof(int) },
                { "integer", typeof(int) }, // VB for int
                { typeof(long).FullName!, typeof(long) },
                { "long", typeof(long) },
                { "int64", typeof(long) },
                { typeof(bool).FullName!, typeof(bool) },
                { "bool", typeof(bool) },
                { "boolean", typeof(bool) }, // VB for bool
                { typeof(string).FullName!, typeof(string) },
                { "string", typeof(string) },
            };

        private static readonly IReadOnlyList<Type> AllowedConstantClrTypes = AllowedConstantTypes.Values.Distinct().ToArray();

        public static bool TryParseLanguage(string? str, out CodeGeneratorLanguage language)
        {
            foreach (var pair in AllowedLanguages.Where(pair => string.Equals(str, pair.Key, StringComparison.InvariantCultureIgnoreCase)))
            {
                language = pair.Value;
                return true;
            }

            language = default;
            return false;
        }

        public static bool IsAllowedConstantType(Type type) => AllowedConstantClrTypes.Contains(type);

        public static bool TryParseConstantValue(string? str, out object? result)
        {
            if (StringUtility.IsNullOrEmpty(str))
            {
                result = null;
                return true;
            }

            if (str.Length > 1 && str[0] == '"' && str[^1] == '"')
            {
#if NETFRAMEWORK
                result = str[1..^1].Replace("\"\"", "\"");
#else
                result = str[1..^1].Replace("\"\"", "\"", StringComparison.Ordinal);
#endif
                return true;
            }

#if NETFRAMEWORK
            var colonPos = str.IndexOf(':');
#else
            var colonPos = str.IndexOf(':', StringComparison.Ordinal);
#endif
            return colonPos < 1
                ? TryParseConstantValueGuessingType(str, out result)
                : TryParseTypedConstantValue(str.Substring(0, colonPos), str[(colonPos + 1)..], out result);
        }

        private static bool TryParseConstantValueGuessingType(string str, out object? result)
        {
            if (int.TryParse(str, out var parsedInt))
            {
                result = parsedInt;
                return true;
            }

            if (long.TryParse(str, out var parsedLong))
            {
                result = parsedLong;
                return true;
            }

            if (bool.TryParse(str, out var parsedBool))
            {
                result = parsedBool;
                return true;
            }

            result = str;
            return true;
        }

        private static bool TryParseTypedConstantValue(string typeStr, string str, out object? result)
        {
            if (!AllowedConstantTypes.TryGetValue(typeStr.Trim(), out var type))
            {
                result = null;
                return false;
            }

            try
            {
                result = Convert.ChangeType(str, type, CultureInfo.InvariantCulture);
                return true;
            }
            catch (Exception e) when (!e.IsCriticalException())
            {
                result = null;
                return false;
            }
        }
    }
}