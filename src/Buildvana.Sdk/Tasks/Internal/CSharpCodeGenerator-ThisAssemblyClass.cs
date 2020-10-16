// -----------------------------------------------------------------------------------
// Copyright (C) Riccardo De Agostini and Buildvana contributors. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
//
// Part of this file may be third-party code, distributed under a compatible license.
// See THIRD-PARTY-NOTICES file in the project root for third-party copyright notices.
// -----------------------------------------------------------------------------------

using System.Globalization;

namespace Buildvana.Sdk.Tasks.Internal
{
    internal partial class CSharpCodeGenerator
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

        protected override void EndInternalStaticClass() => NewLine("}");

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
#pragma warning disable CS8604 // Possible null reference argument (only on net46) - NET46 reference assemblies are not annotated for nullability

                    Text(value.ToString());
#pragma warning restore CS8604
                    break;
            }

            Text(';');
        }
    }
}