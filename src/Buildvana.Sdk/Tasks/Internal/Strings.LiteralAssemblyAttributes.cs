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
        public static class LiteralAssemblyAttributes
        {
            public const string InvalidParameterNameFmt = "BVE1500: The parameter '{0}' has an invalid name.";
            public const string InvalidParameterIndexFmt = "BVE1501: The parameter '{0}' has an invalid parameter index.";
            public const string SkippedNumberedParameterFmt = "BVE1502: The parameter '{0}' was supplied, but not all previously numbered parameters.";
            public const string EmptyParameterFmt = "BVE1503: The parameter '{0}' is empty.";
        }
    }
}