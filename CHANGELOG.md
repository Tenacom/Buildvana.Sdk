# Changelog

All notable changes to the Buildvana SDK will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## Unreleased changes

### New features

- https://github.com/Buildvana/Buildvana.Sdk/pull/124 - When the new `UseAlternatePack` property is set to `true`, Buildvana SDK hijacks the Pack target to publish a project to one or more folders and/or create a setup executable using InnoSetup.

### Changes to existing features

- https://github.com/Buildvana/Buildvana.Sdk/pull/123 - **POTENTIALLY BREAKING CHANGE:** The minimum supported MSBuild version is now 17.0 (.NET SDK 6.0, Visual Studio 2022 v17.0). As a consequence, the only supported .NET environments are .NET 6.0 or newer and .NET Framework 4.7.2 or newer. This of course refers to the build phase; you can use Buildvana SDK to target older versions of .NET, .NET Core, or .NET Framework.
- https://github.com/Buildvana/Buildvana.Sdk/pull/123 - **BREAKING CHANGE:** The `AllowUnderscoresInMemberNames` property is no longer supported. Just append `;CA1707` to the `NoWarn` property instead.

### Bugs fixed in this release

### Known problems introduced by this release

## [1.0.0-alpha.14](https://github.com/Buildvana/Buildvana.Sdk/releases/tag/1.0.0-alpha.14) (2021-09-13)

### Changes to existing features

- Warning NU1604 is no longer suppressed on dependencies automatically introduced in projects by Buildvana SDK. Suppressing the warning prevented a yellow triangle from appearing near the affected packages in Visual Studio 2019 until version 16.7; in version 16.11, on the contrary, the yellow triangle appears if the warning _is_ suppressed.

## [1.0.0-alpha.13](https://github.com/Buildvana/Buildvana.Sdk/releases/tag/1.0.0-alpha.13) (2021-09-12)

### Changes to existing features

- **POTENTIALLY BREAKING CHANGE:** The minimum supported MSBuild version is now 16.8 (.NET SDK 5.0, Visual Studio 2019 v16.8).
- **POTENTIALLY BREAKING CHANGE:** Building with .NET Core 3.1 SDK is not supported any longer.

### Bugs fixed in this release

- https://github.com/Buildvana/Buildvana.Sdk/pull/98 - XML documentation files are now correctly created (regression in versions 1.0.0-alpha.10 through 12).

## [1.0.0-alpha.12](https://github.com/Buildvana/Buildvana.Sdk/releases/tag/1.0.0-alpha.12) (2021-01-19)

### Bugs fixed in this release

- https://github.com/Buildvana/Buildvana.Sdk/pull/74 - Projects using Buildvana SDK now work with Omnisharp in VS Code.

## [1.0.0-alpha.11](https://github.com/Buildvana/Buildvana.Sdk/releases/tag/1.0.0-alpha.11) (2021-01-19)

### Bugs fixed in this release

- https://github.com/Buildvana/Buildvana.Sdk/pull/72 - False-positive BVW1400 and/or BVW1900 warnings are not raised any more.
- https://github.com/Buildvana/Buildvana.Sdk/pull/72 - Properties `GenerateAssemblyCLSCompliantAttribute` and `GenerateAssemblyComVisibleAttribute` are not set any more if `GenerateLiteralAssemblyInfo` is set to `false`.
- https://github.com/Buildvana/Buildvana.Sdk/pull/72 - `LiteralAssemblyAttribute` items are not generated any more if `GenerateLiteralAssemblyInfo` is set to `false`.
- https://github.com/Buildvana/Buildvana.Sdk/pull/72 - Warning CS3021 ("'type' does not need a CLSCompliant attribute because the assembly does not have a CLSCompliant attribute") is not suppressed any more if `GenerateLiteralAssemblyInfo` is set to `false`.
- https://github.com/Buildvana/Buildvana.Sdk/pull/72 - Literal assembly attributes are now correctly regenerated if an attribute's named parameter changes.
- https://github.com/Buildvana/Buildvana.Sdk/pull/72 - `WriteLiteralAssemblyAttributes` and `WriteThisAssemblyClass` tasks are now correctly unloaded after execution.

## [1.0.0-alpha.10](https://github.com/Buildvana/Buildvana.Sdk/releases/tag/1.0.0-alpha.10) (2021-01-03)

### Changes to existing features

- **POTENTIALLY BREAKING CHANGE:** The minimum supported MSBuild version is 16.7 (.NET SDK 3.1, Visual Studio 2019 v16.7).
- **BREAKING CHANGE:** The syntax for parameters of literal assembly attributes, as well as constants in "ThisAssembly" classes, has changed. The new syntax is described in [this document](docs\ConstantsSyntax.md).
- **BREAKING CHANGE:** The `Microsoft.CodeAnalysis.FxCopAnalyzers` package is not imported any more, due to its deprecation in favor of `Microsoft.CodeAnalysis.NetAnalyzers` (see [the relevant documentation](https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/overview) for more details).
- **BREAKING CHANGE:** The `UseStandardAnalyzers` property is not used any more. The new `UseStyleCopAnalyzers` property enables the use of the `StyleCop.Analyzers` package.
- https://github.com/Buildvana/Buildvana.Sdk/pull/62 - Messages listing the icon, license file, and/or third-party copyright notice included in packages are now shown only when packing.
- https://github.com/Buildvana/Buildvana.Sdk/pull/57 - Generated `ThisAssembly` classes now have [CompilerGenerated](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.compilergeneratedattribute) and [ExcludeFromCodeCoverage](https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.codeanalysis.excludefromcodecoverageattribute) attributes.
- **BREAKING CHANGE:** https://github.com/Buildvana/Buildvana.Sdk/pull/57 - The default for the `UseJetBrainsAnnotations` property is now `false`. The reason is that it was counterintuitive to mention JetBrains annotations in projects _not_ using them.
- Compiled tasks used to generate ThisAssembly classes and literal assembly attributes have been completely rewritten using Roslyn code generators.
- The message for error `BVE1004` now reports the minimum required MSBuild version.
- The message for warning `BVW1900` ("ThisAssembly class generation is only supported in C# and Visual Basic projects") now reports the `Language` MSBuild property value for the project.
- **POTENTIALLY BREAKING CHANGE:** Errors `BVE1900` and `BVE1901` did not make sense with [the new constant syntax](docs\ConstantsSyntax.md). They have been removed, and the old error `BVE1902` is now `BVE1900`.

### Bugs fixed in this release

- https://github.com/Buildvana/Buildvana.Sdk/pull/65 - Warning BVW1900 issued on every project with a `<TargetFrameworks>` property and ThisAssembly class generation enabled.

## [1.0.0-alpha.9](https://github.com/Buildvana/Buildvana.Sdk/releases/tag/1.0.0-alpha.9) (2020-10-10)

### Changes to existing features

- https://github.com/Buildvana/Buildvana.Sdk/pull/51 - The automatically-added package reference to `ReSharper.ExportAnnotations.Task` has been updated to version 1.3.1.
- **POTENTIALLY BREAKING CHANGE:** https://github.com/Buildvana/Buildvana.Sdk/pull/51 - The `EnableThisAssemblyClass` property has been renamed to `GenerateThisAssemblyClass` and its default value is now `false`.

### Bugs fixed in this release

- Thanks to the `ReSharper.ExportAnnotations.Task` update, building a project on a non-Windows system will no longer fail. See https://github.com/tenacom/ReSharper.ExportAnnotations/issues/23 for details.

## [1.0.0-alpha.8](https://github.com/Buildvana/Buildvana.Sdk/releases/tag/1.0.0-alpha.8) (2020-10-10)

### Changes to existing features

- https://github.com/Buildvana/Buildvana.Sdk/pull/47 - The automatically-added package reference to `ReSharper.ExportAnnotations.Task` has been updated to version 1.3.0.
- https://github.com/Buildvana/Buildvana.Sdk/pull/49 - Compiled tasks are built for more target frameworks, to cover a larger number of build environments and MSBuild / .NET (Core) / Visual Studio versions.

### Bugs fixed in this release

- Thanks to the `ReSharper.ExportAnnotations.Task` update, building a project with `dotnet build` using .NET Core SDK v3.1 or .NET SDK 5-rc1 does not require.NET Core 2.1 to be installed any longer. See https://github.com/tenacom/ReSharper.ExportAnnotations/issues/20 for details.

## [1.0.0-alpha.7](https://github.com/Buildvana/Buildvana.Sdk/releases/tag/1.0.0-alpha.7) (2020-09-28)

### New features

- https://github.com/Buildvana/Buildvana.Sdk/issues/41 - Buildvana SDK now uses compiled tasks instead of inline tasks, thus improving build performance.
- https://github.com/Buildvana/Buildvana.Sdk/issues/43 - Setting the `EnableDefaultThisAssemblyConstants` property to `false` suppresses creation of default constants in the `ThisAssembly` class.
- Warning [BVW1400] is now issued if literal assembly attribute generation is enabled for a project in a language that is neither C# nor Visual Basic. Previous versions silently skipped the code generation phase.
- Warning [BVW1900] is now issued if `ThisAssembly` class generation is enabled for a project in a language that is neither C# nor Visual Basic. Previous versions silently skipped the code generation phase.

### Changes to existing features

- **POTENTIALLY BREAKING CHANGE:** https://github.com/Buildvana/Buildvana.Sdk/issues/44 - The `AssemblyInfo` module has been removed. Assembly attribute generation-related properties like e.g. `GenerateAssemblyInfo`, `GenerateAssemblyVersionAttribute`, etc. are not set to `true` any more at project and common files evaluation time; instead, they are left unset and defaulted to `true` later.
- **POTENTIALLY BREAKING CHANGE:** [Errors and warnings](docs/ErrorsAndWarnings.md) have been renumbered.
- **BREAKING CHANGE:** https://github.com/Buildvana/Buildvana.Sdk/issues/44 - The `CLSCompliant` property is no longer set to `true` by default; it must be set explicitly in order to generate the respective assembly attribute. Projects that contain `CLSCompliant` attributes on types and members and do not set the `CLSCompliant` property will now issue warning CS3021: *'<type_or_member>' does not need a CLSCompliant attribute because the assembly does not have a CLSCompliant attribute.*. To avoid the warning, set the `CLSCompliant` property to `true` (the previous default) in the project file or in a common file.
- **BREAKING CHANGE:** https://github.com/Buildvana/Buildvana.Sdk/issues/44 - The `ComVisible` property is no longer set to `false` by default; it must be set explicitly in order to generate the respective assembly attribute. In projects that need to have all types and members of the compiled assembly hidden from COM, now you must set the `ComVisible` property to `false` (the previous default) in the project file or in a common file.

### Bugs fixed in this release

- https://github.com/Buildvana/Buildvana.Sdk/issues/42 - The `ThisAssembly` class was never generated by previous versions of Buildvana SDK.

### Known problems introduced by this release

## [1.0.0-alpha.6](https://github.com/Buildvana/Buildvana.Sdk/releases/tag/1.0.0-alpha.6) (2020-09-19)

### New features

- https://github.com/Buildvana/Buildvana.Sdk/issues/35 - A package reference to `Microsoft.NETFramework.ReferenceAssemblies` is automatically added to projects targeting .NET Framework so they can be built on non-Windows systems, or without a .NET Targeting  Pack installed.

### Bugs fixed in this release

- https://github.com/Buildvana/Buildvana.Sdk/issues/36 - Building projects with [centrally-managed package versions](https://stu.dev/managing-package-versions-centrally) now works.

## [1.0.0-alpha.5](https://github.com/Buildvana/Buildvana.Sdk/releases/tag/1.0.0-alpha.5) (2020-09-17)

### Bugs fixed in this release

- Dependency `ReSharper.ExportAnnotations` has been updated to version 1.1.0. This release fixes two rather serious bugs that affected Buildvana SDK's functionality. See [their changelog](https://github.com/tenacom/ReSharper.ExportAnnotations/blob/main/CHANGELOG.md) for more information.

## [1.0.0-alpha.4](https://github.com/Buildvana/Buildvana.Sdk/releases/tag/1.0.0-alpha.4) (2020-09-14)

### Changes to existing features

- https://github.com/Buildvana/Buildvana.Sdk/issues/30 - The LiteralAssemblyAttributes module now works as expected.

## [1.0.0-alpha.3](https://github.com/Buildvana/Buildvana.Sdk/releases/tag/1.0.0-alpha.3) (2020-09-14)

### New features

- **POTENTIALLY BREAKING CHANGE:** https://github.com/Buildvana/Buildvana.Sdk/issues/26 - A unit test project is now recognized as such, by convention, if its name ends with `.Tests`.
  To opt out of this convention, explicitly set `IsTestProject` to `true` or `false`.

### Changes to existing features

- Dependency `StyleCop.Analyzers` has been updated to version 1.2.0-beta.205

## [1.0.0-alpha.2](https://github.com/Buildvana/Buildvana.Sdk/releases/tag/1.0.0-alpha.2) (2020-09-13)

### New features

- https://github.com/Buildvana/Buildvana.Sdk/issues/22 - Warning CA1707 (Identifiers should not contain underscores) is now suppressed by default in test projects. You can control this feature via the `AllowUnderscoresInMemberNames` property.

## [1.0.0-alpha.1](https://github.com/Buildvana/Buildvana.Sdk/releases/tag/1.0.0-alpha.1) (2020-09-12)

Initial release.
