﻿// ---------------------------------------------------------------------------------------
// Copyright (C) Riccardo De Agostini and contributors. All rights reserved.
// Licensed under the MIT license.
// See the LICENSE file in the project root for full license information.
//
// Part of this file may be third-party code, distributed under a compatible license.
// See the THIRD-PARTY-NOTICES file in the project root for third-party copyright notices.
// ---------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Buildvana.Sdk.Utilities;

public static class StringUtility
{
    // Like string.IsNullOrEmpty, but nullability-annotated for all platforms
    // https://github.com/dotnet/roslyn/issues/37995
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullOrEmpty([NotNullWhen(false)] string? str)
        => string.IsNullOrEmpty(str);
}
