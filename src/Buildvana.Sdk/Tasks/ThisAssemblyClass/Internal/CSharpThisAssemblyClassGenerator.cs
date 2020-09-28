// -----------------------------------------------------------------------------------
// Copyright (C) Riccardo De Agostini and Buildvana contributors. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
//
// Part of this file may be third-party code, distributed under a compatible license.
// See THIRD-PARTY-NOTICES file in the project root for third-party copyright notices.
// -----------------------------------------------------------------------------------

using System.Globalization;
using Buildvana.Sdk.Tasks.Internal;

namespace Buildvana.Sdk.Tasks.ThisAssemblyClass.Internal
{
    internal sealed class CSharpThisAssemblyClassGenerator : ThisAssemblyClassGeneratorBase<CSharpCodeBuilder>
    {
        protected override void BeginNamespace(string name)
        {
            if (!StringUtility.IsNullOrEmpty(name))
            {
                NewLine("namespace ");
                Text(name);
                NewLine("{");
            }
        }

        protected override void EndNamespace(string name)
        {
            if (!StringUtility.IsNullOrEmpty(name))
            {
                NewLine("}");
            }
        }

        protected override void BeginInternalStaticClass(string name)
        {
            NewLine("internal static class ");
            Text(name);
            NewLine("{");
        }

        protected override void EndInternalStaticClass(string name) => NewLine("}");

        protected override void PublicConstant(string name, object value)
        {
            var type = value.GetType();
            NewLine("public const ");
            Text(type.FullName!);
            Text(' ');
            Text(name);
            Text(" = ");
            switch (value)
            {
                case string stringValue:
                    QuotedText(stringValue);
                    break;
                case bool boolValue:
                    Text(boolValue ? "true" : "false");
                    break;
                case long longValue:
                    Text(longValue.ToString(CultureInfo.InvariantCulture));
                    Text('L');
                    break;
                default:
                    Text(value.ToString());
                    break;
            }

            Text(';');
        }
    }
}