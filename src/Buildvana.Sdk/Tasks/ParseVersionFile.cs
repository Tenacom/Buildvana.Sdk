// -----------------------------------------------------------------------------------
// Copyright (C) Riccardo De Agostini and Buildvana contributors. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
//
// Part of this file may be third-party code, distributed under a compatible license.
// See THIRD-PARTY-NOTICES file in the project root for third-party copyright notices.
// -----------------------------------------------------------------------------------

using System;
using System.IO;
using Buildvana.Sdk.Internal;
using JetBrains.Annotations;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Buildvana.Sdk.Tasks
{
    public sealed class ParseVersionFile : Task
    {
        [PublicAPI]
        [Required]
        public string? VersionFileFullPath { get; set; }

        [PublicAPI]
        [Output]
        public string? Version { get; private set; }

        [PublicAPI]
        [Output]
        public string? VersionPrefix { get; private set; }

        [PublicAPI]
        [Output]
        public string? VersionSuffix { get; private set; }

        [PublicAPI]
        [Output]
        public string? AssemblyVersion { get; private set; }

        [PublicAPI]
        [Output]
        public string? AssemblyFileVersion { get; private set; }

        [PublicAPI]
        [Output]
        public string? AssemblyInformationalVersion { get; private set; }

        public override bool Execute()
        {
            try
            {
                // Checks on VersionFileFullPath have been made by MSBuild code.
                var versionText = File.ReadAllText(VersionFileFullPath!.Trim()).Trim();
                SemanticVersion semVersion;
                try
                {
                    semVersion = SemanticVersion.Parse(versionText);
                }
                catch (FormatException fex)
                {
                    throw new BuildErrorException("BVE1603: " + fex.Message);
                }

                Version = semVersion.ToString();
                VersionPrefix = $"{semVersion.Major}.{semVersion.Minor}.{semVersion.Patch}";
                VersionSuffix = semVersion.Prerelease;
                AssemblyVersion = $"{semVersion.Major}.0.0.0";
                AssemblyFileVersion = $"{semVersion.Major}.{semVersion.Minor}.{semVersion.Patch}.0";
                AssemblyInformationalVersion = semVersion.ToString();
            }
            catch (BuildErrorException ex)
            {
                Log.LogError(ex.Message);
            }
            catch (Exception e) when (!e.IsFatalException())
            {
                Log.LogErrorFromException(e, true, true, "ParseVersionFile.cs");
            }

            return !Log.HasLoggedErrors;
        }
    }
}