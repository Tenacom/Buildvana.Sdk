# Errors and warnings

- [Overview](#overview)
- [Buildvana SDK core (1000-1099)](#buildvana-sdk-core-1000-1099)
- [Compiled tasks (1100-1199)](#compiled-tasks-1100-1199)
- [AssemblyInfo module (1200-1299)](#assemblyinfo-module-1200-1299)
- [AssemblySigning module (1300-1399)](#assemblysigning-module-1300-1399)
- [JetBrainsAnnotations module (1400-1499)](#jetbrainsannotations-module-1400-1499)
- [LiteralAssemblyAttributes module (1500-1599)](#literalassemblyattributes-module-1500-1599)
- [NuGetPack module (1600-1699)](#nugetpack-module-1600-1699)
- [ParseVersionFile module (1700-1799)](#parseversionfile-module-1700-1799)
- [ReferenceAssemblies module (1800-1899)](#referenceassemblies-module-1800-1899)
- [StandardAnalyzers module (1900-1999)](#standardanalyzers-module-1900-1999)
- [ThisAssemblyClass module (2000-2099)](#thisassemblyclass-module-2000-2099)
- [XmlDocumentation module (2100-2199)](#xmldocumentation-module-2100-2199)

## Overview

All errors defined by the Buildvana SDK have a `BVE` prefix, while warnings have a `BVW` prefix. All error and warning numbers start from 1000, so they have no leading zeros.

Each module, as well as the SDK itself, is assigned a contiguous range of 100 error and 100 warning numbers, as listed below.

## Buildvana SDK core (1000-1099)

| Error code | Message | Description |
| ---------- | ------- | ----------- |
| BVE1000 | Sdk.props not imported. | `Sdk.targets` was imported, but `Sdk.props` was not. |
| BVE1001 | Sdk.targets not imported. | `Sdk.props` was imported, but `Sdk.targets` was not. |
| BVE1002 | Sdk.props and Sdk.targets are in different directories. | The `Version` attributes in the `<Import>` directives for `Sdk.props` and `Sdk.targets` do not match. |
| BVE1003 | Home directory not defined. | No suitable value for the `HomeDirectory` property has been found. |

| Warning code | Message | Description |
| ------------ | ------- | ----------- |
| No warnings defined. |  |  |

## Compiled tasks (1100-1199)

| Error code | Message | Description |
| ---------- | ------- | ----------- |
| BVE1100 | Parameter '...' is missing or empty. | Something went wrong with a Buildvana SDK module. Please [open an issue](https://github.com/Buildvana/Buildvana.Sdk/issues/new/choose). |
| BVE1101 | Language '...' is not supported. | Something went wrong with a Buildvana SDK module. Please [open an issue](https://github.com/Buildvana/Buildvana.Sdk/issues/new/choose). |
| BVE1102 | The file '...' could not be created. | There was an error trying to write to a file. Try cleaning the project and rebuilding it. If the problem arises again, please [open an issue](https://github.com/Buildvana/Buildvana.Sdk/issues/new/choose). |

| Warning code | Message | Description |
| ------------ | ------- | ----------- |
| No warnings defined. |  |  |

## AssemblyInfo module (1200-1299)

| Error code | Message | Description |
| ---------- | ------- | ----------- |
| No errors defined. |  |  |

| Warning code | Message | Description |
| ------------ | ------- | ----------- |
| No warnings defined. |  |  |

## AssemblySigning module (1300-1399)

| Error code | Message | Description |
| ---------- | ------- | ----------- |
| BVE1300 | Cannot extract key from '...'. | Either the specified `.pfx` file is missing, ot the wrong password (or no password) was given. |

| Warning code | Message | Description |
| ------------ | ------- | ----------- |
| No warnings defined. |  |  |

## JetBrainsAnnotations module (1400-1499)

| Error code | Message | Description |
| ---------- | ------- | ----------- |
| No errors defined. |  |  |

| Warning code | Message | Description |
| ------------ | ------- | ----------- |
| No warnings defined. |  |  |

## LiteralAssemblyAttributes module (1500-1599)

| Error code | Message | Description |
| ---------- | ------- | ----------- |
| BVE1500 | The parameter '...' has an invalid name. | A `LiteralAssemblyAttribute` item's metadata has a name starting with `_Parameter`, but what follows could not be parsed as an integer number. |
| BVE1501 | The parameter '...' has an invalid parameter index. | A `LiteralAssemblyAttribute` item's metadata has a name starting with `_Parameter` followed by an integer number lower than 1 (e.g. `_Parameter0` or `_Parameter-5`). |
| BVE1502 | The parameter '...' was supplied, but not all previously numbered parameters. | A `LiteralAssemblyAttribute` item's metadata specifying one or more numbered parameters are not in a 1-based sequence (e.g. there is a `_Parameter2` but no `_Parameter1`, or there are `_Parameter1`  and `_Parameter3` but no `_Parameter2`). |
| BVE1503 | The parameter '...' is empty. | A `LiteralAssemblyAttribute` item's metadata has empty content. To specify the empty string as a parameter, use `""` (e.g. `<_Parameter1>""<_Parameter1>`).

| Warning code | Message | Description |
| ------------ | ------- | ----------- |
| No warnings defined. |  |  |

## NuGetPack module (1600-1699)

| Error code | Message | Description |
| ---------- | ------- | ----------- |
| BVE1600 | Specified license file '...' does not exist. | A license file specified by the `PackageLicensePath` property does not exist. |
| BVE1601 | Specified license file '...' was not found. | A license file specified by the `PackageLicenseFile` property was not found in the project's directory nor in any containing directory. |
| BVE1602 | No license file found. | No license file was specified, and no default license file has been found. Set the `LicenseFileInPackage` property to `false` to explicitly exclude any license file to be included in the package. |
| BVE1603 | Specified third-party notice file '...' does not exist. | A third-party notice file specified by the `PackageThirdPartyNoticePath` property does not exist. |
| BVE1604 | Specified third-party notice file '...' was not found. | A third-party notice file specified by the `PackageThirdPartyNoticeFile` property was not found in the project's directory nor in any containing directory. |
| BVE1605 | No third-party notice file found. | No third-party notice file was specified, and no default third-party notice file has been found. Set the `ThirdPartyNoticeInPackage` property to `false` to explicitly exclude any third-party notice file to be included in the package. |
| BVE1606 | Specified package icon file '...' does not exist. | A package icon file specified by the `PackageIconPath` property does not exist. |
| BVE1607 | Specified package icon file '...' was not found. | A package icon file specified by the `PackageIcon` property was not found in the project's directory nor in any containing directory. |
| BVE1608 | No package icon file found. | No package icon file was specified, and no default package icon has been found. Set the `IconInPackage` property to `false` to explicitly exclude any package icon to be included in the package. |

| Warning code | Message | Description |
| ------------ | ------- | ----------- |
| No warnings defined. |  |  |

## ParseVersionFile module (1700-1799)

| Error code | Message | Description |
| ---------- | ------- | ----------- |
| BVE1700 | Version file '...' not found. | A version file, either specified via the VersionFileName property or the default "VERSION", has not been found. |
| BVE1701 | Version file '...' found outside repository. | A version file, either specified via the VersionFileName property or the default "VERSION", has been found but it is outside of the repository (above the home directory). |
| BVE1702 | Version file '...' not found. | A version file, specified via the VersionFileFullPath property, has not been found. |
| BVE1703 | _(may vary)_ | The version file contains an invalid semantic version. The error message may contain more details. |

| Warning code | Message | Description |
| ------------ | ------- | ----------- |
| No warnings defined. |  |  |

## ReferenceAssemblies module (1800-1899)

| Error code | Message | Description |
| ---------- | ------- | ----------- |
| No errors defined. |  |  |

| Warning code | Message | Description |
| ------------ | ------- | ----------- |
| No warnings defined. |  |  |

## StandardAnalyzers module (1900-1999)

| Error code | Message | Description |
| ---------- | ------- | ----------- |
| No errors defined. |  |  |

| Warning code | Message | Description |
| ------------ | ------- | ----------- |
| No warnings defined. |  |  |

## ThisAssemblyClass module (2000-2099)

| Error code | Message | Description |
| ---------- | ------- | ----------- |
| BVE2000 | Constant '...' has no '...' metadata. | The specified `ThisAssemblyConstant` item is missing required metadata. |
| BVE2001 | Constant '...' has unknown or unsupported type '...'. | The specified `ThisAssemblyConstant` item contains an invalid `Type` metadata. |
| BVE2002 | Constant '...' has invalid value '...'. | The specified `ThisAssemblyConstant` item contains a `Value` metadata that cannot be successfully parsed as the type indicated by the `Type` metadata of the same item. |

| Warning code | Message | Description |
| ------------ | ------- | ----------- |
| No warnings defined. |  |  |

## XmlDocumentation module (2100-2199)

| Error code | Message | Description |
| ---------- | ------- | ----------- |
| No errors defined. |  |  |

| Warning code | Message | Description |
| ------------ | ------- | ----------- |
| No warnings defined. |  |  |
