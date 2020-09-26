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
    internal partial class Strings
    {
        public static class ThisAssemblyClass
        {
            public const string MissingConstantMetadataFmt = "BVE1900: Constant '{0}' has no '{1}' metadata.";
            public const string UnsupportedConstantTypeFmt = "BVE1901: Constant '{0}' has unknown or unsupported type '{1}'.";
            public const string InvalidConstantValueFmt = "BVE1902: Constant '{0}' has invalid value '{1}'.";
        }
    }
}