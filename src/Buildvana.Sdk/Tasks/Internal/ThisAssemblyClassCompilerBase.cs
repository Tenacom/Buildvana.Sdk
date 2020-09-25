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
using System.Text;
using Buildvana.Sdk.Internal;
using Microsoft.Build.Framework;

using static Buildvana.Sdk.Tasks.Internal.Strings.ThisAssemblyClass;

namespace Buildvana.Sdk.Tasks.Internal
{
    internal abstract class ThisAssemblyClassCompilerBase : IThisAssemblyClassCompiler
    {
        private static readonly IReadOnlyDictionary<string, Type> PredefinedConstantTypes
            = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                { "byte", typeof(byte) },
                { "short", typeof(short) },
                { "int", typeof(int) },
                { "Integer", typeof(int) }, // VB for int
                { "long", typeof(long) },
                { "bool", typeof(bool) },
                { "Boolean", typeof(bool) }, // VB for bool
                { "string", typeof(string) },
            };

        private static readonly IReadOnlyList<Type> ClrConstantTypes = PredefinedConstantTypes.Values.Distinct().ToArray();

        protected ThisAssemblyClassCompilerBase(WriteThisAssemblyClass task)
        {
            Namespace = task.Namespace;
            ClassName = StringUtility.IsNullOrEmpty(task.ClassName) ? "ThisAssembly" : task.ClassName;
            Constants = (task.Constants ?? Enumerable.Empty<ITaskItem>()).Select(c =>
            {
                var name = c.ItemSpec.Trim();
                var typeStr = c.GetMetadata("Type").Trim();
                if (StringUtility.IsNullOrEmpty(typeStr))
                {
                    throw new BuildErrorException(MissingConstantMetadataFmt, name, "Type");
                }

                if (!TryDetermineConstantType(typeStr, out var type))
                {
                    throw new BuildErrorException(UnsupportedConstantTypeFmt, name, typeStr);
                }

                var valueStr = c.GetMetadata("Value").Trim();
                object value;
                try
                {
                    value = Convert.ChangeType(valueStr, type, CultureInfo.InvariantCulture);
                }
                catch (Exception e) when (e is InvalidCastException || e is FormatException || e is OverflowException)
                {
                    throw new BuildErrorException(InvalidConstantValueFmt, name, valueStr);
                }

                return (Name: name, Value: value);
            }).ToList();
        }

        protected string Namespace { get; }

        protected string ClassName { get; }

        private IReadOnlyList<(string Name, object Value)> Constants { get; }

        public void CompileThisClass(StringBuilder sb)
        {
            AppendBeforeConstants(sb);
            foreach (var constant in Constants)
            {
                AppendConstant(sb, constant);
            }

            AppendAfterConstants(sb);
        }

        protected abstract void AppendBeforeConstants(StringBuilder sb);

        protected abstract void AppendAfterConstants(StringBuilder sb);

        protected abstract void AppendConstant(StringBuilder sb, (string Name, object Value) constant);

        private static bool TryDetermineConstantType(string typeMetadata, out Type? type)
        {
            if (PredefinedConstantTypes.TryGetValue(typeMetadata, out type))
            {
                return true;
            }

            foreach (var clrType in ClrConstantTypes.Where(t => string.Equals(t.FullName, typeMetadata, StringComparison.OrdinalIgnoreCase)))
            {
                type = clrType;
                return true;
            }

            type = null;
            return false;
        }
    }
}