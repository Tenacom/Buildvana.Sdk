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
using Buildvana.Sdk.CodeGeneration.Configuration;
using Buildvana.Sdk.Tasks.Resources;
using Buildvana.Sdk.Utilities;
using JetBrains.Annotations;
using Microsoft.Build.Framework;

namespace Buildvana.Sdk.Tasks.ThisAssemblyClass
{
    public sealed class WriteThisAssemblyClass : BuildvanaSdkCodeGeneratorTask
    {
        [PublicAPI]
        public string? ClassName { get; set; }

        [PublicAPI]
        public string? Namespace { get; set; }

        [PublicAPI]
        public ITaskItem[]? Constants { get; set; }

        protected override IEnumerable<CodeFragment> GetCodeFragments()
        {
            var classNamespace = StringUtility.IsNullOrEmpty(Namespace) ? null : Namespace;
            var className = StringUtility.IsNullOrEmpty(ClassName) ? "ThisAssembly" : ClassName;
            var constants = (Constants ?? Enumerable.Empty<ITaskItem>()).Select(ExtractConstantDefinitionFromItem);
            var fragment = new CodeGeneration.Configuration.ThisAssemblyClass(classNamespace, className, constants);
            yield return fragment;
        }

        private static Constant ExtractConstantDefinitionFromItem(ITaskItem item)
        {
            var name = item.ItemSpec.Trim();
            var valueStr = item.GetMetadata("Value")?.Trim() ?? "<null>";
            if (!CodeGeneratorConfigurationUtility.TryParseConstantValue(valueStr, out var value))
            {
                throw new BuildErrorException(Strings.ThisAssemblyClass.InvalidConstantValueFmt, name, valueStr);
            }

            return new Constant(name, value);
        }
    }
}