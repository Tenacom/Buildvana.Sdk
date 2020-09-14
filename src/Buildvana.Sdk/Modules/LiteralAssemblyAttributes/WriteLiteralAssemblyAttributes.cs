#line 2 "WriteLiteralAssemblyAttributes.cs"

using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Shared;
using Microsoft.Build.Utilities;

namespace Buildvana.Tasks
{
    internal static class Messages
    {
        public const string ErrorFmt_UnsupportedLanguage = "BVE1400: Language '{0}' is not supported.";
        public const string Error_MissingOutputPath = "BVE1401: The 'OutputPath' parameter is missing or empty.";
        public const string ErrorFmt_CouldNotWriteOutput = "BVE1402: The file '{0}' could not be created. {1}";
        public const string ErrorFmt_InvalidParameterName = "BVE1403: The parameter '{0}' has an invalid name.";
        public const string ErrorFmt_InvalidParameterIndex = "BVE1404: The parameter '{0}' has an invalid parameter index.";
        public const string ErrorFmt_SkippedNumberedParameter = "BVE1405: The parameter '{0}' was supplied, but not all previously numbered parameters.";
        public const string ErrorFmt_EmptyParameter = "BVE1406: The parameter '{0}' is empty.";
    }

    public class WriteLiteralAssemblyAttributes : Task
    {
        static readonly IReadOnlyDictionary<string, Func<AttributeCompilerBase>> AttributeCompilers
            = new Dictionary<string, Func<AttributeCompilerBase>> {
                { "C#", () => new CSharpAttributeCompiler() },
                { "VB", () => new VisualBasicAttributeCompiler() },
            };

        [Required]
        public string Language { get; set; }

        public ITaskItem[] LiteralAssemblyAttributes { get; set; }

        [Required]
        public string OutputPath { get; set; }

        public override bool Execute()
        {
            try
            {
                if (LiteralAssemblyAttributes.Length == 0)
                {
                    return true;
                }

                Language = Language ?? string.Empty;
                if (!AttributeCompilers.TryGetValue(Language, out var createCompiler))
                {
                    throw new BuildErrorException(Messages.ErrorFmt_UnsupportedLanguage, Language);
                }

                if (string.IsNullOrEmpty(OutputPath))
                {
                    throw new BuildErrorException(Messages.Error_MissingOutputPath);
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
                catch (Exception ex) when (IsIORelatedException(ex))
                {
                    throw new BuildErrorException(Messages.ErrorFmt_CouldNotWriteOutput, OutputPath ?? string.Empty, ex.Message);
                }
            }
            catch (BuildErrorException ex)
            {
                Log.LogError(ex.Message);
            }
            catch (Exception e)
            {
                Log.LogErrorFromException(e, true, true, "WriteLiteralAssemblyAttributes.cs");
            }

            return !Log.HasLoggedErrors;
        }

        static bool IsIORelatedException(Exception e)
        {
            // These all derive from IOException:
            //   * DirectoryNotFoundException
            //   * DriveNotFoundException
            //   * EndOfStreamException
            //   * FileLoadException
            //   * FileNotFoundException
            //   * PathTooLongException
            //   * PipeException
            return e is UnauthorizedAccessException
                   || e is NotSupportedException
                   || (e is ArgumentException && !(e is ArgumentNullException))
                   || e is SecurityException
                   || e is IOException;
        }
    }

    sealed class BuildErrorException : Exception
    {
        public BuildErrorException(string message)
            : base(message) { }

        public BuildErrorException(string format, params object[] args)
            : base(string.Format(CultureInfo.InvariantCulture, format, args)) { }
    }

    abstract class AttributeCompilerBase
    {
        public abstract string Extension { get; }

        public void AppendAttribute(StringBuilder sb, ITaskItem attributeItem)
        {
            // Some attributes only allow positional constructor arguments, or the user may just prefer them.
            // To set those, use metadata names like "_Parameter1", "_Parameter2" etc.
            // If a parameter index is skipped, it's an error.
            var customMetadata = attributeItem.CloneCustomMetadata();

            var orderedParameters = new List<string>(customMetadata.Count + 1 /* max possible slots needed */);
            var namedParameters = new Dictionary<string, string>();

            foreach (DictionaryEntry entry in customMetadata)
            {
                var name = (string)entry.Key;
                var value = (string)entry.Value;

                if (string.IsNullOrWhiteSpace(value))
                    throw new BuildErrorException(Messages.ErrorFmt_EmptyParameter, name);

                value = TransformParameterValue(value.Trim());

                if (name.StartsWith("_Parameter", StringComparison.OrdinalIgnoreCase))
                {
                    if (!Int32.TryParse(name.Substring("_Parameter".Length), out int index))
                        throw new BuildErrorException(Messages.ErrorFmt_InvalidParameterName, name);

                    if (index < 1)
                        throw new BuildErrorException(Messages.ErrorFmt_InvalidParameterIndex, name);

                    if (index > orderedParameters.Count + 1)
                        throw new BuildErrorException(Messages.ErrorFmt_SkippedNumberedParameter, index);

                    // "_Parameter01" and "_Parameter1" would overwrite each other.
                    if (index > orderedParameters.Count) {
                        orderedParameters.Add(value);
                    }
                    else
                    {
                        orderedParameters[index - 1] = value;
                    }
                }
                else
                {
                    namedParameters.Add(name, value);
                }
            }

            var encounteredNull = false;
            for (int i = 0; i < orderedParameters.Count; i++)
            {
                if (orderedParameters[i] == null)
                {
                    encounteredNull = true;
                }
                else if (encounteredNull)
                {
                    throw new BuildErrorException(Messages.ErrorFmt_SkippedNumberedParameter, i + 1);
                }
            }
            
            AppendAttributeCore(sb, attributeItem.ItemSpec, orderedParameters, namedParameters);
        }

        protected abstract void AppendAttributeCore(StringBuilder sb, string attributeType, IReadOnlyList<string> orderedParameters, IReadOnlyDictionary<string, string> namedParameters);

        protected abstract string TransformParameterValue(string value);
    }

    sealed class CSharpAttributeCompiler : AttributeCompilerBase
    {
        public override string Extension => ".cs";

        protected override void AppendAttributeCore(StringBuilder sb, string attributeType, IReadOnlyList<string> orderedParameters, IReadOnlyDictionary<string, string> namedParameters)
        {
            sb.Append("[assembly:").Append(attributeType);
            
            if (orderedParameters.Count > 0 || namedParameters.Count > 0)
            {
                sb.Append('(');
                var first = true;
                
                foreach (var parameter in orderedParameters)
                {
                    if (first)
                        first = false;
                    else
                        sb.Append(", ");

                    sb.Append(parameter);
                }

                foreach (var pair in namedParameters)
                {
                    if (first)
                        first = false;
                    else
                        sb.Append(", ");

                    sb.Append(pair.Key).Append('=').Append(pair.Value);
                }

                sb.Append(')');
            }

            sb.Append(']').AppendLine();
        }

        protected override string TransformParameterValue(string value)
        {
            switch (value.ToUpperInvariant())
            {
                case "TRUE": return "true";
                case "FALSE": return "false";
                case "NULL": return "null";
                default: return value;
            }
        }
    }

    sealed class VisualBasicAttributeCompiler : AttributeCompilerBase
    {
        public override string Extension => ".vb";

        protected override void AppendAttributeCore(StringBuilder sb, string attributeType, IReadOnlyList<string> orderedParameters, IReadOnlyDictionary<string, string> namedParameters)
        {
            sb.Append("<Assembly: ").Append(attributeType);
            
            if (orderedParameters.Count > 0 || namedParameters.Count > 0)
            {
                sb.Append('(');
                var first = true;
                
                foreach (var parameter in orderedParameters)
                {
                    if (first)
                        first = false;
                    else
                        sb.Append(", ");

                    sb.Append(parameter);
                }

                foreach (var pair in namedParameters)
                {
                    if (first)
                        first = false;
                    else
                        sb.Append(", ");

                    sb.Append(pair.Key).Append(":=").Append(pair.Value);
                }

                sb.Append(')');
            }

            sb.Append('>').AppendLine();
        }

        protected override string TransformParameterValue(string value)
        {
            switch (value.ToUpperInvariant())
            {
                case "TRUE": return "True";
                case "FALSE": return "False";
                case "NULL": return "Nothing";
                default: return value;
            }
        }
    }
}