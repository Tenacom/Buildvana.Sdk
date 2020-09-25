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
    public class WriteThisAssemblyClass : Task
    {
        private static readonly IReadOnlyDictionary<string, Func<WriteThisAssemblyClass, IThisAssemblyClassCompiler>>
            ThisAssemblyClassCompilers
                = new Dictionary<string, Func<WriteThisAssemblyClass, IThisAssemblyClassCompiler>>
                {
                    { "C#", task => new CSharpThisAssemblyClassCompiler(task) },
                    { "VB", task => new VisualBasicThisAssemblyClassCompiler(task) },
                };

        private string _language = string.Empty;
        private string _outputPath = string.Empty;
        private string _className = string.Empty;
        private string _namespace = string.Empty;
        private ITaskItem[] _constants = Array.Empty<ITaskItem>();

        [PublicAPI]
        [Required]
        public string Language
        {
            get => _language;
            set => _language = value ?? string.Empty;
        }

        [PublicAPI]
        [Required]
        public string OutputPath
        {
            get => _outputPath;
            set => _outputPath = value ?? string.Empty;
        }

        [PublicAPI]
        public string ClassName
        {
            get => _className;
            set => _className = value ?? string.Empty;
        }

        [PublicAPI]
        public string Namespace
        {
            get => _namespace;
            set => _namespace = value ?? string.Empty;
        }

#pragma warning disable CA1819 // Properties should not return arrays - Required by MSBuild API
        [PublicAPI]
        public ITaskItem[] Constants
        {
            get => _constants;
            set => _constants = value ?? Array.Empty<ITaskItem>();
        }
#pragma warning restore CA1819

        public override bool Execute()
        {
            try
            {
                if (!ThisAssemblyClassCompilers.TryGetValue(Language, out var createCompiler))
                {
                    throw new BuildErrorException(Strings.Common.UnsupportedLanguageFmt, Language);
                }

                if (StringUtility.IsNullOrEmpty(OutputPath))
                {
                    throw new BuildErrorException(Strings.Common.MissingParameterFmt, nameof(OutputPath));
                }

                var compiler = createCompiler(this);
                var sb = new StringBuilder();
                compiler.CompileThisClass(sb);

                try
                {
                    // Overwrites file if it already exists (and can be overwritten)
                    File.WriteAllText(OutputPath, sb.ToString(), Encoding.UTF8);
                }
                catch (Exception e) when (e.IsIORelatedException())
                {
                    throw new BuildErrorException(Strings.Common.CouldNotWriteFileFmt, OutputPath, e.Message);
                }
            }
            catch (BuildErrorException e)
            {
                Log.LogError(e.Message);
            }
            catch (Exception e) when (!e.IsFatalException())
            {
                Log.LogErrorFromException(e, true, true, "WriteThisAssemblyClass.cs");
            }

            return !Log.HasLoggedErrors;
        }
    }
}