// -----------------------------------------------------------------------------------
// Copyright (C) Riccardo De Agostini and Buildvana contributors. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
//
// Part of this file may be third-party code, distributed under a compatible license.
// See THIRD-PARTY-NOTICES file in the project root for third-party copyright notices.
// -----------------------------------------------------------------------------------

using System.Collections.Generic;
using Buildvana.Sdk.Tasks.Internal;

namespace Buildvana.Sdk.Tasks.ThisAssemblyClass.Internal
{
    internal abstract class ThisAssemblyClassGeneratorBase<TBuilder> : CodeGenerator<TBuilder>, IThisAssemblyClassGenerator
        where TBuilder : CodeBuilder, new()
    {
        public string GenerateCode(string classNamespace, string className, IEnumerable<ConstantDefinition> constants)
        {
            BeginNamespace(classNamespace);
            BeginInternalStaticClass(className);
            foreach (var constant in constants)
            {
                PublicConstant(constant.Name, constant.Value);
            }

            EndInternalStaticClass();
            EndNamespace(classNamespace);
            return GetGeneratedCode();
        }

        protected abstract void BeginNamespace(string name);

        protected abstract void EndNamespace(string name);

        protected abstract void BeginInternalStaticClass(string name);

        protected abstract void EndInternalStaticClass();

        protected abstract void PublicConstant(string name, object value);
    }
}