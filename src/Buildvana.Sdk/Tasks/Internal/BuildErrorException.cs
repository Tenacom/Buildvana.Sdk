// -----------------------------------------------------------------------------------
// Copyright (C) Riccardo De Agostini and Buildvana contributors. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
//
// Part of this file may be third-party code, distributed under a compatible license.
// See THIRD-PARTY-NOTICES file in the project root for third-party copyright notices.
// -----------------------------------------------------------------------------------

using System;
using System.Globalization;

namespace Buildvana.Sdk.Tasks.Internal
{
#pragma warning disable CA1032 // Implement standard exception constructors - Not needed since this class is internal
#pragma warning disable CA1064 // Exceptions should be public - This does not need to be public
    internal sealed class BuildErrorException : Exception
#pragma warning restore CA1064
#pragma warning restore CA1032
    {
        public BuildErrorException(string message)
            : base(message)
        {
        }

        public BuildErrorException(string format, params object[] args)
            : base(string.Format(CultureInfo.InvariantCulture, format, args))
        {
        }
    }
}