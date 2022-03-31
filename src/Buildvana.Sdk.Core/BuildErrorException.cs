// ---------------------------------------------------------------------------------------
// Copyright (C) Riccardo De Agostini and contributors. All rights reserved.
// Licensed under the MIT license.
// See the LICENSE file in the project root for full license information.
//
// Part of this file may be third-party code, distributed under a compatible license.
// See the THIRD-PARTY-NOTICES file in the project root for third-party copyright notices.
// ---------------------------------------------------------------------------------------

using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Buildvana.Sdk;

[Serializable]
public sealed class BuildErrorException : Exception
{
    public BuildErrorException()
    {
        // ReSharper doesn't like empty constructors
    }

    public BuildErrorException(string message)
        : base(message)
    {
        // ReSharper doesn't like empty constructors
    }

    public BuildErrorException(string format, params object[] args)
        : base(string.Format(CultureInfo.InvariantCulture, format, args))
    {
        // ReSharper doesn't like empty constructors
    }

    public BuildErrorException(string message, Exception innerException)
        : base(message, innerException)
    {
        // ReSharper doesn't like empty constructors
    }

    private BuildErrorException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        // ReSharper doesn't like empty constructors
    }
}
