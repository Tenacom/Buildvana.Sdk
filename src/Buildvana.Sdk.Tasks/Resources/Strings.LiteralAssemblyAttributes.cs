// ---------------------------------------------------------------------------------------
// Copyright (C) Riccardo De Agostini and contributors. All rights reserved.
// Licensed under the MIT license.
// See the LICENSE file in the project root for full license information.
//
// Part of this file may be third-party code, distributed under a compatible license.
// See the THIRD-PARTY-NOTICES file in the project root for third-party copyright notices.
// ---------------------------------------------------------------------------------------

namespace Buildvana.Sdk.Tasks.Resources;

internal partial class Strings
{
    public static class LiteralAssemblyAttributes
    {
        public const string InvalidParameterNameFmt = "BVE1400: The parameter '{0}' has an invalid name.";
        public const string InvalidParameterIndexFmt = "BVE1401: The parameter '{0}' has an invalid parameter index.";
        public const string SkippedNumberedParameterFmt = "BVE1402: The parameter '{0}' was supplied, but not all previously numbered parameters.";
        public const string InvalidParameterValueFmt = "BVE1403: The parameter '{0}' cannot be parsed.";
    }
}
