﻿// Copyright (C) Tenacom and Contributors. Licensed under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;

namespace Buildvana.Sdk;

public readonly struct Undefined : IEquatable<Undefined>
{
    public static Undefined Value => default;

    public static bool operator ==(Undefined first, Undefined second) => true;

    public static bool operator !=(Undefined first, Undefined second) => false;

    public bool Equals(Undefined other) => true;

    public override bool Equals(object? obj) => obj is Undefined;

    public override int GetHashCode() => 0;

    public override string ToString() => string.Empty;
}
