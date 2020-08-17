#line 2 "WriteThisAssemblyClass.cs"

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
    public class WriteThisAssemblyClass : TaskExtension
    {
        private const string ErrorFmt_UnsupportedLanguage = "BVE1800: Language '{0}' is not supported.";
        private const string Error_MissingOutputPath = "BVE1801: The 'OutputPath' parameter is missing or empty.";
        private const string ErrorFmt_CouldNotWriteOutput = "BVE1802: The file '{0}' could not be created. {1}";
        private const string ErrorFmt_MissingConstantMetadata = "BVE1803: Constant '{0}' has no '{1}' metadata.";
        private const string ErrorFmt_UnsupportedConstantType = "BVE1804: Constant '{0}' has unknown or unsupported type '{1}'.";
        private const string ErrorFmt_InvalidConstantValue = "BVE1805: Constant '{0}' has invalid value '{1}'.";

        private static readonly IReadOnlyDictionary<string, Func<ThisAssemblyClassCompilerBase>> ThisAssemblyClassCompilers
            = new Dictionary<string, Func<WriteThisAssemblyClass, ThisAssemblyClassCompilerBase>> {
                { "C#", task => new CSharpThisAssemblyClassCompiler(task) },
                { "VB", task => new VisualBasicThisAssemblyClassCompiler(task) },
            };

        [Required]
        public string Language { get; set; }

        [Required]
        public string OutputPath { get; set; }

        public string ClassName { get; set; }

        public string Namespace { get; set; }

        public ITaskItem[] Constants { get; set; }

        public override bool Execute()
        {
            try
            {
                Language = Language ?? string.Empty;
                if (!ThisAssemblyClassCompilers.TryGetValue(Language, out var createCompiler))
                {
                    throw new BuildErrorException(ErrorFmt_UnsupportedLanguage, Language);
                }

                if (string.IsNullOrEmpty(OutputPath))
                {
                    throw new BuildErrorException(Error_MissingOutputPath);
                }
    
                var compiler = createCompiler();
                var sb = new StringBuilder();
                compiler.CompileThisClass(sb);
                    
                try
                {
                    File.WriteAllText(OutputPath, sb.ToString(), Encoding.UTF8); // Overwrites file if it already exists (and can be overwritten)
                }
                catch (Exception ex) when (IsIORelatedException(ex))
                {
                    throw new BuildErrorException(ErrorFmt_CouldNotWriteOutput, OutputPath ?? string.Empty, ex.Message);
                }
            }
            catch (BuildErrorException ex)
            {
                Log.LogError(ex.Message);
            }
            catch (Exception e)
            {
                Log.LogErrorFromException(e, true, true, "WriteThisAssemblyClass.cs");
            }

            return !Log.HasLoggedErrors;
        }

        private static bool IsIORelatedException(Exception e)
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

    internal sealed class BuildErrorException : Exception
    {
        public BuildErrorException(string message)
            : base(message) { }

        public BuildErrorException(string format, params object[] args)
            : base(string.Format(CultureInfo.InvariantCulture, format, args)) { }
    }

    internal abstract class ThisAssemblyClassCompilerBase
    {
        private readonly static IReadOnlyDictionary<string, Type> PredefinedConstantTypes =
            new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                { "byte", typeof(byte) },
                { "short", typeof(short) },
                { "int", typeof(int) },
                { "Integer", typeof(int) }, // VB for int
                { "long", typeof(long) },
                { "bool", typeof(bool) },
                { "Boolean", typeof(bool) }, // VB for bool
                { "string", typeof(string) },
            };

        private readonly static IReadOnlyList<type> ClrConstantTypes = PredefinedConstantTypes.Values.Distinct().ToArray();

        public ThisAssemblyClassCompilerBase(WriteThisAssemblyClass task)
        {
            Namespace = string.IsNullOrEmpty(task.Namespace) ? null : task.Namespace;
            ClassName = string.IsNullOrEmpty(task.ClassName) ? "ThisAssembly" : task.ClassName;
            Constants = (task.Constants ?? Enumerable.Empty<ITaskItem>).Select(c => {
                var name = c.ItemSpec.Trim();
                var typeStr = c.GetMetadata("Type").Trim();
                if (string.IsNullOrEmpty(typeStr))
                {
                    throw new BuildErrorException(ErrorFmt_MissingConstantMetadata, name, "Type");
                }

                if (!TryDetermineConstantType(typeStr, out var type))
                {
                    throw new BuildErrorException(ErrorFmt_UnsupportedConstantType, name, typeStr);
                }

                var valueStr = c.GetMetadata("Value").Trim();
                object value;
                try
                {
                    value = Convert.ChangeType(valueStr, type);
                }
                catch (Exception e) when (e is InvalidCastException || e is FormatException || e is OverflowException)
                {
                    throw new BuildErrorException(ErrorFmt_InvalidConstantValue, name, valueStr);
                }

                return new ConstantDefinition {
                    Name = name,
                    Value = value
                };
            }).ToList();
        }

        protected string Namespace { get; }

        protected string ClassName { get; }

        protected IReadOnlyList<ConstantDefinition> Constants { get; }

        public void CompileThisClass(StringBuilder sb)
        {
            AppendBeforeConstants(sb);
            foreach (var constant in Constants)
            {
                AppendConstant(sb, constant);
            }

            AppendAfterConstants(sb);
        }

        protected abstract void AppendBeforeConstants(StringBuilder sb);

        protected abstract void AppendAfterConstants(StringBuilder sb);

        protected abstract void AppendConstant(StringBuilder sb, ConstantDefinition constant);

        private static bool TryDetermineConstantType(string typeMetadata, out Type type)
        {
            if (PredefinedConstantTypes.TryGetValue(typeMetadata, out type))
            {
                return true;
            }

            foreach (var clrType in ClrConstantTypes)
            {
                if (clrType.FullName.Equals(typeMetadata, StringComparison.OrdinalIgnoreCase))
                {
                    type = clrType;
                    return true;
                }
            }

            type = null;
            return false;
        }
    }

    internal sealed class CSharpThisAssemblyClassCompiler : ThisAssemblyClassCompilerBase
    {
        public CSharpThisAssemblyClassCompiler(WriteThisAssemblyClass task)
            : base(task)
        {
        }

        protected override void AppendBeforeConstants(StringBuilder sb)
        {
            sb.AppendLine("// <auto-generated />");
            if (!string.IsNullOrEmpty(Namespace))
            {
                sb.Append("namespace ").Append(Namespace).AppendLine(" {");
            }

            sb.Append("internal partial class ").Append(ClassName).AppendLine(" {");
        }

        protected override void AppendAfterConstants(StringBuilder sb)
        {
            sb.Append('}');
            if (!string.IsNullOrEmpty(Namespace))
            {
                sb.Append('}');
            }

        }

        protected override void AppendConstant(StringBuilder sb, ConstantDefinition constant)
        {
            var type = constant.Value.GetType();
            sb.Append("public const ").Append(type.FullName).Append(' ').Append(constant.Name).Append(" = ");
            switch (type)
            {
                case typeof(string):
                    AppendQuoted(sb, (string)constant.Value);
                    break;
                case typeof(bool):
                    sb.Append((bool)constant.Value ? "true" : "false");
                    break;
                case typeof(long):
                    sb.Append(constant.Value).Append('L');
                    break;
                default:
                    sb.Append(constant.Value);
                    break;
            }

            sb.AppendLine(";");
        }

        private void AppendQuoted(StringBuilder sb, string str)
        {
            sb.Append("@\"");
            var length = str.Length;
            if (length > 0)
            {
                var position = 0;
                while (position < length)
                {
                    nextPosition = str.IndexOf('"', position);
                    if (nextPosition < 0)
                    {
                        sb.Append(str, position, length - position);
                        position = length;
                    }
                    else if (nextPosition == position)
                    {
                        sb.Append("\"\"");
                        position++;
                    }
                    else
                    {
                        sb.Append(str, position, nextPosition - position).Append("\"\"");
                        position = nextPosition + 1;
                    }
                }
            }

            sb.Append('"');
        }
    }

    internal sealed class VisualBasicThisAssemblyClassCompiler : ThisAssemblyClassCompilerBase
    {
        public VisualBasicThisAssemblyClassCompiler(WriteThisAssemblyClass task)
            : base(task)
        {
        }

        protected override void AppendBeforeConstants(StringBuilder sb)
        {
            sb.AppendLine("' <auto-generated/>");
                
            sb.Append("Namespace Global");
            if (!string.IsNullOrEmpty(Namespace))
            {
                sb.Append('.').Append(Namespace);
            }

            sb.AppendLine();

            sb.Append("Friend NotInheritable Partial Class ").AppendLine(ClassName)
                .AppendLine("Public Sub New()").AppendLine("End Sub");
        }

        protected override void AppendAfterConstants(StringBuilder sb)
        {
            sb.AppendLine("End Class").AppendLine("End Namespace");
        }

        protected override void AppendConstant(StringBuilder sb, ConstantDefinition constant)
        {
            var type = constant.Value.GetType();
            sb.Append("Public Const ").Append(constant.Name).Append(" As ").Append(type.FullName).Append(" = ");
            switch (type)
            {
                case typeof(string):
                    AppendQuoted(sb, (string)constant.Value);
                    break;
                case typeof(long):
                    sb.Append(constant.Value).Append('L');
                    break;
                default:
                    sb.Append(constant.Value);
                    break;
            }

            sb.AppendLine();
        }

        private void AppendQuoted(StringBuilder sb, string str)
        {
            sb.Append('"');
            var length = str.Length;
            if (length > 0)
            {
                var position = 0;
                while (position < length)
                {
                    nextPosition = str.IndexOf('"', position);
                    if (nextPosition < 0)
                    {
                        sb.Append(str, position, length - position);
                        position = length;
                    }
                    else if (nextPosition == position)
                    {
                        sb.Append("\"\"");
                        position++;
                    }
                    else
                    {
                        sb.Append(str, position, nextPosition - position).Append("\"\"");
                        position = nextPosition + 1;
                    }
                }
            }

            sb.Append('"');
        }
    }
 
    internal sealed class ConstantDefinition
    {
        public string Name;

        public object Value;
    }
}