// -----------------------------------------------------------------------------------
// Copyright (C) Riccardo De Agostini and Buildvana contributors. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
//
// Part of this file may be third-party code, distributed under a compatible license.
// See THIRD-PARTY-NOTICES file in the project root for third-party copyright notices.
// -----------------------------------------------------------------------------------

namespace Buildvana.Sdk.Tasks.Internal
{
    internal partial class VisualBasicCodeGenerator
    {
        protected override string TransformLiteralConstant(string value)
            => value.ToUpperInvariant() switch {
                "TRUE" => "True",
                "FALSE" => "False",
                "NULL" => "Nothing",
                _ => value,
            };
    }
}