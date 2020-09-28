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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Buildvana.Sdk.Tasks.Internal;
using Buildvana.Sdk.Tasks.Resources;
using Buildvana.Sdk.Tasks.ThisAssemblyClass.Internal;
using JetBrains.Annotations;
using Microsoft.Build.Framework;

namespace Buildvana.Sdk.Tasks.ThisAssemblyClass
{
    public class WriteThisAssemblyClass : BuildvanaSdkTask
    {
        private static readonly IReadOnlyDictionary<string, Type> PredefinedConstantTypes
            = new Dictionary<string, Type>(StringComparer.Ordinal)
            {
                { "byte", typeof(byte) },
                { "Byte", typeof(byte) }, // VB for byte
                { "short", typeof(short) },
                { "Short", typeof(short) }, // VB for short
                { "int", typeof(int) },
                { "Integer", typeof(int) }, // VB for int
                { "long", typeof(long) },
                { "Long", typeof(long) }, // VB for long
                { "bool", typeof(bool) },
                { "Boolean", typeof(bool) }, // VB for bool
                { "string", typeof(string) },
                { "String", typeof(string) }, // VB for string
            };

        private static readonly IReadOnlyList<Type> ClrConstantTypes = PredefinedConstantTypes.Values.Distinct().ToArray();

        [PublicAPI]
        [Required]
        public string? Language { get; set; }

        [PublicAPI]
        [Required]
        public string? OutputPath { get; set; }

        [PublicAPI]
        public string? ClassName { get; set; }

        [PublicAPI]
        public string? Namespace { get; set; }

#pragma warning disable CA1819 // Properties should not return arrays - Required by MSBuild API
        [PublicAPI]
        public ITaskItem[]? Constants { get; set; }
#pragma warning restore CA1819

        protected override void Run()
        {
            Language ??= string.Empty;
            if (!ThisAssemblyClassGenerator.TryCreate(Language, out var generator))
            {
                throw new BuildErrorException(Strings.UnsupportedLanguageFmt, Language);
            }

            if (StringUtility.IsNullOrEmpty(OutputPath))
            {
                throw new BuildErrorException(Strings.MissingParameterFmt, nameof(OutputPath));
            }

            var classNamespace = Namespace ?? string.Empty;
            var className = StringUtility.IsNullOrEmpty(ClassName) ? "ThisAssembly" : ClassName;
            var constants = (Constants ?? Enumerable.Empty<ITaskItem>())
                .Select(ExtractConstantDefinitionFromItem)
                .ToArray();

            var code = generator.GenerateCode(classNamespace, className, constants);

            try
            {
                // Overwrites file if it already exists (and can be overwritten)
                File.WriteAllText(OutputPath, code, Encoding.UTF8);
            }
            catch (Exception e) when (e.IsIORelatedException())
            {
                throw new BuildErrorException(Strings.CouldNotWriteFileFmt, OutputPath, e.Message);
            }
        }

        private static bool TryDetermineConstantType(string typeMetadata, [MaybeNullWhen(false)] out Type type)
        {
            if (PredefinedConstantTypes.TryGetValue(typeMetadata, out type))
            {
                return true;
            }

            type = ClrConstantTypes.FirstOrDefault(t => string.Equals(t.FullName, typeMetadata, StringComparison.Ordinal));
            return type != null;
        }

        private (string Name, object Value) ExtractConstantDefinitionFromItem(ITaskItem item)
        {
            var name = item.ItemSpec.Trim();
            var typeStr = item.GetMetadata("Type").Trim();
            if (StringUtility.IsNullOrEmpty(typeStr))
            {
                throw new BuildErrorException(Strings.ThisAssemblyClass.MissingConstantMetadataFmt, name, "Type");
            }

            if (!TryDetermineConstantType(typeStr, out var type))
            {
                throw new BuildErrorException(Strings.ThisAssemblyClass.UnsupportedConstantTypeFmt, name, typeStr);
            }

            var valueStr = item.GetMetadata("Value")?.Trim() ?? string.Empty;
            object value;
            try
            {
                value = Convert.ChangeType(valueStr, type, CultureInfo.InvariantCulture);
            }
            catch (Exception e) when (e is InvalidCastException || e is FormatException || e is OverflowException)
            {
                throw new BuildErrorException(Strings.ThisAssemblyClass.InvalidConstantValueFmt, name, valueStr);
            }

            return (name, value);
        }
    }
}