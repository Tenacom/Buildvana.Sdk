﻿// Copyright (C) Tenacom and Contributors. Licensed under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Buildvana.Sdk.Tasks;

[Serializable]
public sealed class BuildErrorException : Exception
{
    public BuildErrorException()
    {
    }

    public BuildErrorException(string message)
        : base(message)
    {
    }

    public BuildErrorException(string format, params object[] args)
        : base(string.Format(CultureInfo.InvariantCulture, format, args))
    {
    }

    public BuildErrorException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    private BuildErrorException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
