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
using System.IO;
using System.Text;
using Buildvana.Sdk.Internal;
using Buildvana.Sdk.Tasks.Internal;
using JetBrains.Annotations;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Buildvana.Sdk.Tasks
{
    public sealed class WriteLiteralAssemblyAttributes : Task
    {
        private static readonly IReadOnlyDictionary<string, Func<IAttributeCompiler>> AttributeCompilers
            = new Dictionary<string, Func<IAttributeCompiler>>
            {
                { "C#", () => new CSharpAttributeCompiler() },
                { "VB", () => new VisualBasicAttributeCompiler() },
            };

        [PublicAPI]
        [Required]
        public string? Language { get; set; }

        [PublicAPI]
#pragma warning disable CA1819 // Properties should not return arrays - Required by MSBuild API
        public ITaskItem[]? LiteralAssemblyAttributes { get; set; }
#pragma warning restore CA1819

        [PublicAPI]
        [Required]
        public string? OutputPath { get; set; }

        public override bool Execute()
        {
            try
            {
                LiteralAssemblyAttributes ??= Array.Empty<ITaskItem>();
                if (LiteralAssemblyAttributes.Length == 0)
                {
                    return true;
                }

                Language ??= string.Empty;
                if (!AttributeCompilers.TryGetValue(Language, out var createCompiler))
                {
                    throw new BuildErrorException(Strings.Common.UnsupportedLanguageFmt, Language);
                }

                OutputPath ??= string.Empty;
                if (StringUtility.IsNullOrEmpty(OutputPath))
                {
                    throw new BuildErrorException(Strings.Common.MissingParameterFmt, nameof(OutputPath));
                }

                var compiler = createCompiler();
                var sb = new StringBuilder();
                foreach (var attribute in LiteralAssemblyAttributes)
                {
                    compiler.AppendAttribute(sb, attribute);
                }

                try
                {
                    File.WriteAllText(OutputPath, sb.ToString(), Encoding.UTF8); // Overwrites file if it already exists (and can be overwritten)
                }
                catch (Exception e) when (e.IsIORelatedException())
                {
                    throw new BuildErrorException(Strings.Common.CouldNotWriteFileFmt, OutputPath, e.Message);
                }
            }
            catch (BuildErrorException ex)
            {
                Log.LogError(ex.Message);
            }
            catch (Exception e) when (!e.IsFatalException())
            {
                Log.LogErrorFromException(e, true, true, "WriteLiteralAssemblyAttributes.cs");
            }

            return !Log.HasLoggedErrors;
        }
    }
}