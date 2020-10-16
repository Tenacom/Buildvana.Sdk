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
    internal partial class VisualBasicCodeGenerator
    {
        protected override void BeginNamespace(string name)
        {
            NewLine("Namespace Global");
            if (!StringUtility.IsNullOrEmpty(name))
            {
                Text('.');
                Text(name);
            }
        }

        protected override void EndNamespace(string name) => NewLine("End Namespace");

        protected override void BeginInternalStaticClass(string name)
        {
            NewLine("Friend NotInheritable Partial Class ");
            Text(name);
            NewLine("Public Sub New()");
            NewLine("End Sub");
        }

        protected override void EndInternalStaticClass() => NewLine("End Class");

        protected override void PublicConstant(string name, object value)
        {
            var type = value.GetType();
            NewLine("Public Const ");
            Text(name);
            Text(" As ");
            Text(type.FullName!);
            Text(" = ");
            switch (value)
            {
                case string stringValue:
                    QuotedText(stringValue);
                    break;
                case bool boolValue:
                    Text(boolValue ? "True" : "False");
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