// Copyright (C) Tenacom and contributors. Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

#nullable enable

// =============================================================================================
// Setup and Teardown, common to all scripts
// =============================================================================================

Setup<BuildData>(context => new BuildData(context));

Teardown<BuildData>((context, data) =>
{
    // For some reason, DotNetBuildServerShutdown hangs in a GitHub Actions runner;
    // it is still useful on a local machine though.
    // TODO: Test whether it can be enabled in e.g. GitLab CI.
    if (data.CIPlatform is CIPlatform.None)
    {
        context.DotNetBuildServerShutdown(new DotNetBuildServerShutdownSettings
        {
            Razor = true,
            VBCSCompiler = true,
        });
    }
});
