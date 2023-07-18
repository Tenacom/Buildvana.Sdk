# Diagnostics

- [Overview](#overview)
- [Buildvana SDK core (1000-1099)](#buildvana-sdk-core-1000-1099)
- [Source generators (1100-1199)](#source-generators-1100-1199)
- [AssemblySigning module (1200-1299)](#assemblysigning-module-1200-1299)
- [JetBrainsAnnotations module (1300-1399)](#jetbrainsannotations-module-1300-1399)
- [AdditionalAssemblyInfo module (1400-1499)](#additionalassemblyinfo-module-1400-1499)
- [NuGetPack module (1500-1599)](#nugetpack-module-1500-1599)
- [ReferenceAssemblies module (1600-1699)](#referenceassemblies-module-1600-1699)
- [StandardAnalyzers module (1700-1799)](#standardanalyzers-module-1700-1799)
- [XmlDocumentation module (1800-1899)](#xmldocumentation-module-1800-1899)
- [AlternatePack module (1900-1999)](#alternatepack-module-1900-1999)
- [NerdbankGitVersioning module (2000-2099)](#nerdbankgitversioning-module-2000-2099)

## Overview

All diagnostics issued by Buildvana SDK have a `BVSDK` prefix. All numbers start from 1000, so there are no leading zeros.

Each module is assigned a contiguous range of 100 diagnostics, as listed below. The first range are reserved for the SDK itself and for source generators.

## Buildvana SDK core (1000-1099)

| Code | Severity | Message | Description |
| -----| :------: | ------- | ----------- |
| BVSDK1000 | Error | Sdk.props not imported. | `Sdk.targets` was imported, but `Sdk.props` was not. |
| BVSDK1001 | Error | Sdk.targets not imported. | `Sdk.props` was imported, but `Sdk.targets` was not. |
| BVSDK1002 | Error | Sdk.props and Sdk.targets are in different directories. | The `Version` attributes in the `<Import>` directives for `Sdk.props` and `Sdk.targets` do not match. |
| BVESDK003 | Error | Home directory not defined. | No suitable value for the `HomeDirectory` property has been found. |
| BVSDK1004 | Error | Buildvana SDK requires at least MSBuild v... | You are trying to use Buildvana SDK with an unsupported version of MSBuild. See [here](../README.md#toolchain) for a list of supported MSBuild versions. |

## Source generators (1100-1199)

| Code | Severity | Message | Description |
| -----| :------: | ------- | ----------- |
| This module has no associated diagnostics. |  |  |  |

## AssemblySigning module (1200-1299)

| Code | Severity | Message | Description |
| -----| :------: | ------- | ----------- |
| BVSDK1200 | Error | Cannot extract key from '...'. | Either the specified `.pfx` file is missing, ot the wrong password (or no password) was given. |
| BVSDK1201 | Error | '...' does not contain a RSA private key. | The specified `.pfx` file does not contain a RSA private key to export. |

## JetBrainsAnnotations module (1300-1399)

| Code | Severity | Message | Description |
| -----| :------: | ------- | ----------- |
| This module has no associated diagnostics. |  |  |  |

## AdditionalAssemblyInfo module (1400-1499)

| Code | Severity | Message | Description |
| -----| :------: | ------- | ----------- |
| BVSDK1400 | Warning | Additional assembly info generation is not supported for language '...'. | Property `GenerateAdditionalAssemblyInfo` was set to `true` in a project in a language that is neither C# nor Visual Basic. Additional assembly info will not be generated. |

## NuGetPack module (1500-1599)

| Code | Severity | Message | Description |
| -----| :------: | ------- | ----------- |
| BVSDK1500 | Error | Specified license file '...' does not exist. | A license file specified by the `PackageLicensePath` property does not exist. |
| BVSDK1501 | Error | Specified license file '...' was not found. | A license file specified by the `PackageLicenseFile` property was not found in the project's directory nor in any containing directory. |
| BVSDK1502 | Error | No license file found. | No license file was specified, and no default license file has been found. Set the `LicenseFileInPackage` property to `false` to explicitly exclude any license file to be included in the package. |
| BVSDK1503 | Error | Specified third-party notice file '...' does not exist. | A third-party notice file specified by the `PackageThirdPartyNoticePath` property does not exist. |
| BVSDK1504 | Error | Specified third-party notice file '...' was not found. | A third-party notice file specified by the `PackageThirdPartyNoticeFile` property was not found in the project's directory nor in any containing directory. |
| BVSDK1505 | Error | No third-party notice file found. | No third-party notice file was specified, and no default third-party notice file has been found. Set the `ThirdPartyNoticeInPackage` property to `false` to explicitly exclude any third-party notice file to be included in the package. |
| BVSDK1506 | Error | Specified package icon file '...' does not exist. | A package icon file specified by the `PackageIconPath` property does not exist. |
| BVSDK1507 | Error | Specified package icon file '...' was not found. | A package icon file specified by the `PackageIcon` property was not found in the project's directory nor in any containing directory. |
| BVSDK1508 | Error | No package icon file found. | No package icon file was specified, and no default package icon has been found. Set the `IconInPackage` property to `false` to explicitly exclude any package icon to be included in the package. |
| BVSDK1509 | Error | Specified README file '...' does not exist. | A README file specified by the `PackageReadmePath` property does not exist. |
| BVSDK1510 | Error | Specified README file '...' was not found. | A README file specified by the `PackageReadmeFile` property was not found in the project's directory nor in any containing directory. |
| BVSDK1511 | Error | No README file found for package. | No README file was specified, and no default README file has been found. Set the `ReadmeFileInPackage` property to `false` to explicitly exclude any README file to be included in the package. |

## ReferenceAssemblies module (1600-1699)

| Code | Severity | Message | Description |
| -----| :------: | ------- | ----------- |
| This module has no associated diagnostics. |  |  |  |

## StandardAnalyzers module (1700-1799)

| Code | Severity | Message | Description |
| -----| :------: | ------- | ----------- |
| This module has no associated diagnostics. |  |  |  |

## XmlDocumentation module (1800-1899)

| Code | Severity | Message | Description |
| -----| :------: | ------- | ----------- |
| This module has no associated diagnostics. |  |  |  |

## AlternatePack module (1900-1999)

| Code | Severity | Message | Description |
| -----| :------: | ------- | ----------- |
| BVSDK1900 | Error | Unknown alternate pack method '...'. | The `AlternatePackMethod` property was set to an unrecognized value. |

## NerdbankGitVersioning module (2000-2099)

| Code | Severity | Message | Description |
| -----| :------: | ------- | ----------- |
| BVSDK2000 | Error | Version specification JSON file not found. | A `version.json` or `.version.json` file for the project was not found within the repository root. |
