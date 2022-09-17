// ---------------------------------------------------------------------------------------
// Copyright (C) Riccardo De Agostini and contributors. All rights reserved.
// Licensed under the MIT license.
// See the LICENSE file in the project root for full license information.
//
// Part of this file may be third-party code, distributed under a compatible license.
// See the THIRD-PARTY-NOTICES file in the project root for third-party copyright notices.
// ---------------------------------------------------------------------------------------

using System;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Buildvana.Sdk.SourceGenerators.Internal;

internal static class AnalyzerConfigOptionsProviderExtensions
{
    public static bool? GetBooleanMSBuildProperty(this AnalyzerConfigOptionsProvider @this, string name)
        => @this.GlobalOptions.TryGetValue($"build_property.{name}", out var value) ? value.Equals("true", StringComparison.OrdinalIgnoreCase) : null;
}
