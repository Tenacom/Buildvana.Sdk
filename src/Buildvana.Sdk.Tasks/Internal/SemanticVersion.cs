// -----------------------------------------------------------------------------------
// Copyright (C) Riccardo De Agostini and Buildvana contributors. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
//
// Part of this file may be third-party code, distributed under a compatible license.
// See THIRD-PARTY-NOTICES file in the project root for third-party copyright notices.
// -----------------------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace Buildvana.Sdk.Tasks.Internal
{
    internal sealed class SemanticVersion
    {
        // https://semver.org/#is-there-a-suggested-regular-expression-regex-to-check-a-semver-string
        // https://regex101.com/r/Ly7O1x/3/
        // Adapted from PHP PCRE: ?P<group> -> ?<group>
#pragma warning disable SA1000 // new should be followed by a space - StyleCop doesn't understand new C# 9 syntax
        private static readonly Regex SemVerRegex = new(
#pragma warning restore SA1000
            @"^(?<major>0|[1-9]\d*)\." +
            @"(?<minor>0|[1-9]\d*)\." +
            @"(?<patch>0|[1-9]\d*)" +
            @"(?:-(?<prerelease>(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?" +
            @"(?:\+(?<buildmetadata>[0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?$",
            RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

        private SemanticVersion(int major, int minor = 0, int patch = 0, string prerelease = "", string build = "")
        {
            Major = major;
            Minor = minor;
            Patch = patch;
            Prerelease = prerelease;
            Build = build;
        }

        [PublicAPI]
        public int Major { get; }

        [PublicAPI]
        public int Minor { get; }

        [PublicAPI]
        public int Patch { get; }

        [PublicAPI]
        public string Prerelease { get; }

        [PublicAPI]
        public string Build { get; }

        [PublicAPI]
        public static SemanticVersion Parse(string version)
            => TryParseInternal(version, out var result, out var exception)
                ? result
                : throw exception;

        [PublicAPI]
        public static bool TryParse(string version, [MaybeNullWhen(false)] out SemanticVersion result)
            => TryParseInternal(version, out result, out _);

        public override string ToString()
        {
            var sb = new StringBuilder()
                .Append(Major)
                .Append('.')
                .Append(Minor)
                .Append('.')
                .Append(Patch);

            if (Prerelease.Length > 0)
            {
                _ = sb.Append('-').Append(Prerelease);
            }

            if (Build.Length > 0)
            {
                _ = sb.Append('+').Append(Build);
            }

            return sb.ToString();
        }

        private static bool TryParseInternal(string version, [MaybeNullWhen(false)] out SemanticVersion result, [MaybeNullWhen(true)] out Exception exception)
        {
            result = null;
            exception = null;

            var match = SemVerRegex.Match(version);
            if (!match.Success)
            {
                exception = new FormatException($"Invalid version: '{version}' is not a SemVer-compliant version.");
                return false;
            }

            if (!int.TryParse(match.Groups["major"].Value, NumberStyles.None, CultureInfo.InvariantCulture, out var major))
            {
                exception = new FormatException("Invalid version: major version is not a non-negative integer number.");
                return false;
            }

            var minorMatch = match.Groups["minor"];
            if (!minorMatch.Success)
            {
                exception = new FormatException("Invalid version: no minor version given.");
                return false;
            }

            if (!int.TryParse(minorMatch.Value, NumberStyles.None, CultureInfo.InvariantCulture, out var minor))
            {
                exception = new FormatException("Invalid version: minor version is not a non-negative integer number.");
                return false;
            }

            var patchMatch = match.Groups["patch"];
            if (!patchMatch.Success)
            {
                exception = new FormatException("Invalid version: no patch number given.");
                return false;
            }

            if (!int.TryParse(patchMatch.Value, NumberStyles.None, CultureInfo.InvariantCulture, out var patch))
            {
                exception = new FormatException("Invalid version: patch number is not a non-negative integer number.");
                return false;
            }

            var prerelease = match.Groups["prerelease"].Value;
            var build = match.Groups["buildmetadata"].Value;

            result = new SemanticVersion(major, minor, patch, prerelease, build);
            return true;
        }
    }
}
