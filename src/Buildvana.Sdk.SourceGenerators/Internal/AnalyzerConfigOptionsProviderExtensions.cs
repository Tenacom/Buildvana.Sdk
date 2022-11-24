// Copyright (C) Tenacom and Contributors. Licensed under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Buildvana.Sdk.SourceGenerators.Internal;

internal static class AnalyzerConfigOptionsProviderExtensions
{
    public static bool? GetBooleanMSBuildProperty(this AnalyzerConfigOptionsProvider @this, string name)
        => @this.GlobalOptions.TryGetValue($"build_property.{name}", out var value) ? value.Equals("true", StringComparison.OrdinalIgnoreCase) : null;
}
