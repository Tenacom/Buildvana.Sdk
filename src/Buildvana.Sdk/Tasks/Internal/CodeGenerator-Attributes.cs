// -----------------------------------------------------------------------------------
// Copyright (C) Riccardo De Agostini and Buildvana contributors. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
//
// Part of this file may be third-party code, distributed under a compatible license.
// See THIRD-PARTY-NOTICES file in the project root for third-party copyright notices.
// -----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Buildvana.Sdk.Tasks.Internal
{
    internal partial class CodeGenerator
    {
        protected abstract void GenerateAttribute(
            AttributeModifier modifier,
            string type,
            IReadOnlyCollection<string> orderedParameters,
            IReadOnlyDictionary<string, string> namedParameters);

        private void GenerateAttribute(AttributeModifier modifier, string type)
            => GenerateAttribute(modifier, type, Array.Empty<string>(), ImmutableDictionary<string, string>.Empty);
    }
}