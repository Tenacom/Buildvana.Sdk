// -----------------------------------------------------------------------------------
// Copyright (C) Riccardo De Agostini and Buildvana contributors. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
//
// Part of this file may be third-party code, distributed under a compatible license.
// See THIRD-PARTY-NOTICES file in the project root for third-party copyright notices.
// -----------------------------------------------------------------------------------

using System;
using Buildvana.Sdk.Tasks.Internal;
using Microsoft.Build.Utilities;

namespace Buildvana.Sdk.Tasks
{
    public abstract class BuildvanaSdkTask : Task
    {
        public sealed override bool Execute()
        {
            try
            {
                Run();
            }
            catch (BuildErrorException ex)
            {
                Log.LogError(ex.Message);
            }
            catch (Exception e) when (!e.IsFatalException())
            {
                Log.LogErrorFromException(e, true, true, GetType().Name + ".cs");
            }

            return !Log.HasLoggedErrors;
        }

        protected abstract void Run();
    }
}