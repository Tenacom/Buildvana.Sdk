// ---------------------------------------------------------------------------------------
// Copyright (C) Riccardo De Agostini and contributors. All rights reserved.
// Licensed under the MIT license.
// See the LICENSE file in the project root for full license information.
//
// Part of this file may be third-party code, distributed under a compatible license.
// See the THIRD-PARTY-NOTICES file in the project root for third-party copyright notices.
// ---------------------------------------------------------------------------------------

using System;

namespace Buildvana.Sdk.CodeGeneration.Configuration;

public class Constant
{
    public Constant(string name, object? value)
    {
        if (value != null)
        {
            var type = value.GetType();
            if (!CodeGeneratorConfigurationUtility.IsAllowedConstantType(type))
            {
                throw new ArgumentException($"{type.FullName} is not an allowed type for a constant in a 'ThisAssembly' class.");
            }
        }

        Name = name;
        Value = value;
    }

    public string Name { get; }

    public object? Value { get; }
}
