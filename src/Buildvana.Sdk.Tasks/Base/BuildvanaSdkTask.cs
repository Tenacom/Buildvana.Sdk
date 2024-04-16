// Copyright (C) Tenacom and Contributors. Licensed under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using Buildvana.Sdk.Tasks.Internal;
using Microsoft.Build.Utilities;

namespace Buildvana.Sdk.Tasks.Base;

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
