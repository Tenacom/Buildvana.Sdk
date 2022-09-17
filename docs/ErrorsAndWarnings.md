# Errors and warnings

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

## Overview

All errors defined by the Buildvana SDK have a `BVE` prefix, while warnings have a `BVW` prefix. All error and warning numbers start from 1000, so they have no leading zeros.

Each module is assigned a contiguous range of 100 error and 100 warning numbers, as listed below. The first two ranges are reserved for the SDK itself and for source generators.

## Buildvana SDK core (1000-1099)

| Error code | Message | Description |
| ---------- | ------- | ----------- |
| BVE1000 | Sdk.props not imported. | `Sdk.targets` was imported, but `Sdk.props` was not. |
| BVE1001 | Sdk.targets not imported. | `Sdk.props` was imported, but `Sdk.targets` was not. |
| BVE1002 | Sdk.props and Sdk.targets are in different directories. | The `Version` attributes in the `<Import>` directives for `Sdk.props` and `Sdk.targets` do not match. |
| BVE1003 | Home directory not defined. | No suitable value for the `HomeDirectory` property has been found. |
| BVE1004 | Buildvana SDK requires at least MSBuild v... | You are trying to use Buildvana SDK with an unsupported version of MSBuild. See [here](../README.md#toolchain) for a list of supported MSBuild versions. |

| Warning code | Message | Description |
| ------------ | ------- | ----------- |
| No warnings defined. |  |  |

## Source generators (1100-1199)

| Error code | Message | Description |
| ---------- | ------- | ----------- |
| No errors defined. |  |  |

| Warning code | Message | Description |
| ------------ | ------- | ----------- |
| No warnings defined. |  |  |

## AssemblySigning module (1200-1299)

| Error code | Message | Description |
| ---------- | ------- | ----------- |
| BVE1200 | Cannot extract key from '...'. | Either the specified `.pfx` file is missing, ot the wrong password (or no password) was given. |
| BVE1201 | '...' does not contain a RSA private key. | The specified `.pfx` file does not contain a RSA private key to export. |

| Warning code | Message | Description |
| ------------ | ------- | ----------- |
| No warnings defined. |  |  |

## JetBrainsAnnotations module (1300-1399)

| Error code | Message | Description |
| ---------- | ------- | ----------- |
| No errors defined. |  |  |

| Warning code | Message | Description |
| ------------ | ------- | ----------- |
| No warnings defined. |  |  |

## AdditionalAssemblyInfo module (1400-1499)

| Error code | Message | Description |
| ---------- | ------- | ----------- |
| No errors defined. |  |  |

| Warning code | Message | Description |
| ------------ | ------- | ----------- |
| BVW1400 | Additional assembly info generation is not supported for language '...'. | Property `GenerateAdditionalAssemblyInfo` was set to `true` in a project in a language that is neither C# nor Visual Basic. Additional assembly info will not be generated. |

## NuGetPack module (1500-1599)

| Error code | Message | Description |
| ---------- | ------- | ----------- |
| BVE1500 | Specified license file '...' does not exist. | A license file specified by the `PackageLicensePath` property does not exist. |
| BVE1501 | Specified license file '...' was not found. | A license file specified by the `PackageLicenseFile` property was not found in the project's directory nor in any containing directory. |
| BVE1502 | No license file found. | No license file was specified, and no default license file has been found. Set the `LicenseFileInPackage` property to `false` to explicitly exclude any license file to be included in the package. |
| BVE1503 | Specified third-party notice file '...' does not exist. | A third-party notice file specified by the `PackageThirdPartyNoticePath` property does not exist. |
| BVE1504 | Specified third-party notice file '...' was not found. | A third-party notice file specified by the `PackageThirdPartyNoticeFile` property was not found in the project's directory nor in any containing directory. |
| BVE1505 | No third-party notice file found. | No third-party notice file was specified, and no default third-party notice file has been found. Set the `ThirdPartyNoticeInPackage` property to `false` to explicitly exclude any third-party notice file to be included in the package. |
| BVE1506 | Specified package icon file '...' does not exist. | A package icon file specified by the `PackageIconPath` property does not exist. |
| BVE1507 | Specified package icon file '...' was not found. | A package icon file specified by the `PackageIcon` property was not found in the project's directory nor in any containing directory. |
| BVE1508 | No package icon file found. | No package icon file was specified, and no default package icon has been found. Set the `IconInPackage` property to `false` to explicitly exclude any package icon to be included in the package. |
| BVE1509 | Specified README file '...' does not exist. | A README file specified by the `PackageReadmePath` property does not exist. |
| BVE1510 | Specified README file '...' was not found. | A README file specified by the `PackageReadmeFile` property was not found in the project's directory nor in any containing directory. |
| BVE1511 | No README file found for package. | No README file was specified, and no default README file has been found. Set the `ReadmeFileInPackage` property to `false` to explicitly exclude any README file to be included in the package. |

| Warning code | Message | Description |
| ------------ | ------- | ----------- |
| No warnings defined. |  |  |

## ReferenceAssemblies module (1600-1699)

| Error code | Message | Description |
| ---------- | ------- | ----------- |
| No errors defined. |  |  |

| Warning code | Message | Description |
| ------------ | ------- | ----------- |
| No warnings defined. |  |  |

## StandardAnalyzers module (1700-1799)

| Error code | Message | Description |
| ---------- | ------- | ----------- |
| No errors defined. |  |  |

| Warning code | Message | Description |
| ------------ | ------- | ----------- |
| No warnings defined. |  |  |

## XmlDocumentation module (1800-1899)

| Error code | Message | Description |
| ---------- | ------- | ----------- |
| No errors defined. |  |  |

| Warning code | Message | Description |
| ------------ | ------- | ----------- |
| No warnings defined. |  |  |

## AlternatePack module (1900-1999)

| Error code | Message | Description |
| ---------- | ------- | ----------- |
| No errors defined. |  |  |

| Warning code | Message | Description |
| ------------ | ------- | ----------- |
| No warnings defined. |  |  |
