# Buildvana SDK

Part of [the Buildvana project](https://github.com/Buildvana/Buildvana).

[![License](https://img.shields.io/github/license/Buildvana/Buildvana.Sdk.svg)](https://github.com/Buildvana/Buildvana.Sdk/blob/main/LICENSE)
[![GitHub release (latest by date including pre-releases)](https://img.shields.io/github/v/release/Buildvana/Buildvana.Sdk?include_prereleases)](https://github.com/Buildvana/Buildvana.Sdk/releases)
[![Changelog](https://img.shields.io/badge/changelog-Keep%20a%20Changelog%20v1.0.0-%23E05735)](https://github.com/Buildvana/Buildvana.Sdk/blob/main/CHANGELOG.md)

[![CodeFactor](https://www.codefactor.io/repository/github/buildvana/buildvana.sdk/badge)](https://www.codefactor.io/repository/github/buildvana/buildvana.sdk)
[![Last commit](https://img.shields.io/github/last-commit/Buildvana/Buildvana.Sdk.svg)](https://github.com/Buildvana/Buildvana.Sdk/commits/main)
[![Open issues](https://img.shields.io/github/issues-raw/Buildvana/Buildvana.Sdk.svg?label=open+issues)](https://github.com/Buildvana/Buildvana.Sdk/issues?q=is%3Aissue+is%3Aopen+sort%3Aupdated-desc)
[![Closed issues](https://img.shields.io/github/issues-closed-raw/Buildvana/Buildvana.Sdk.svg?label=closed+issues)](https://github.com/Buildvana/Buildvana.Sdk/issues?q=is%3Aissue+is%3Aclosed+sort%3Aupdated-desc)

[![Slack](https://img.shields.io/badge/join_us-on_Slack-ff7fc0.svg?logo=slack)](https://join.slack.com/t/buildvana/shared_invite/zt-e667rvy8-hCtADFiuF8OuiYvthIiWVw)

---

- [At a glance](#at-a-glance)
  - [Benefits](#benefits)
  - [Supported project types](#supported-project-types)
  - [Requirements](#requirements)
    - [Git repository](#git-repository)
    - [Toolchain](#toolchain)
- [Quick start](#quick-start)
- [Contributing](#contributing)
- [Credits](#credits)

---

![Buildvana SDK](https://raw.githubusercontent.com/Buildvana/Buildvana/main/graphics/Buildvana-Readme.png)

**Buildvana** _/bɪldˈvɑːnə/_

1. a transcendent state of the .NET programmer's mind, in which there is neither suffering, confusion, nor waste of time.

2. a collection of open-source methods and processes to help .NET programmers achieve Buildvana.

3. a collection of open-source projects supporting programmers that aim to reach Buildvana.

---

## At a glance

Buildvana SDK is an opinionated, best-practices-based, CI-friendly, VS-friendly, MSBuild-powered build system for .NET projects.

It is part of [the Buildvana project](https://github.com/Buildvana/), which also comprises ready-to-use template repositories that implement automated build, testing, and deployment for .NET projects.

> **DISCLAIMER:** Buildvana SDK, just as the whole Buildvana project, is still a work in progress.  
Your mileage may vary, if you break your build you own both pieces, and so on and so forth.  
However, Buildvana SDK has already been used successfully in production, for both business and open source projects.

### Benefits

- Single source of truth for assembly versions
- Single source of truth for package licenses and copyright notices
- Helps you keep your project files clean and concise - even better than "plain" MSBuild SDKs
- More auto-generated assembly information (`ClsCompliant`, `COMVisible`)
- Custom auto-generated assembly information
- Automatic configuration of commonly-used code analyzers
- ...

### Supported project types

- :heart: Multi-platform / Cross-platform projects
- :heart: Libraries
- :heart: Console apps
- :heart: Windows Forms
- :heart: Projects using `Microsoft.Build.NoTargets` SDK
- :heart: [Avalonia](https://avaloniaui.net) (still experimenting, but no problems so far)
- :question: WPF (testers welcome)
- :question: [UNO Platform](https://platform.uno) (testers welcome)
- :question: All kinds of Web projects (testers welcome)
- :thumbsdown: Legacy (non-SDK) projects

### Supported languages

- :heart: C#
- :heart: Visual Basic
- :yellow_heart: F# (some features disabled)
- :thumbsdown: other languages

### Requirements

#### Git repository

Buildvana SDK assumes that your project is in a Git repository. Furthermore, if your project is packable (i.e. it is meant to be distributed as a NuGet package), Buildvana SDK assumes that it is hosted on a GitHub public repository, for the purposes of generating a symbol package with SourceLink support.

Support for other Git servers will be implemented if / when asked for. It should be trivial enough, as long as there is a suitable SourceLink support package available.

Private repositories may or may not work. Testers welcome.

#### Toolchain

Building via the following tools is supported:

- MSBuild v17.0 or newer (`msbuild`) running under .NET Framework 4.7.2 or newer / .NET 6.0 or newer
- Visual Studio 2022 v17.0 or newer (building from IDE)
- .NET SDK 6.0 or newer (`dotnet build`, `dotnet msbuild`, etc.)

Latest versions of Visual Studio for Mac _should_ work, but we need confirmation by someone with a Mac.

## Quick start

**TODO**

## Contributing

**TODO**

## Proudly built using Buildvana SDK

- [PolyKit](https://github.com/Buildvana/PolyKit), itself a part of the Buildvana project, is both a polyfill library _and_ a polyfill construction kit for C# projects.
- [L.o.U.I.S.](https://github.com/Tenacom/Louis) is a general-purpose library, providing both polyfills (via PolyKit) and commonly-used types, suitable for multi-platform libraries and applications.
- [Cecil.XmlDocNames](https://github.com/Tenacom/Cecil.XmlDocNames) is a tiny library that generates XML-documentation-compliant names for [Mono.Cecil](https://github.com/jbevain/cecil) objects.
- [ReSharper.ExportAnnotations](https://github.com/Tenacom/ReSharper.ExportAnnotations) lets you distribute ReSharper code annotations in XML format along with your libraries, without keeping a transient dependency on the [JetBrains.Annotations](https://www.nuget.org/packages/JetBrains.Annotations) package.
- Practically every one of [Tenacom](https://github.com/Tenacom)'s private projects since the first preview of Buildvana SDK: almost 50 libraries, a bunch of console apps, some WinForms apps, even an Avalonia app (with more coming soon).
## Credits

The peaceful octopus logo is a modified version of [Peace](https://thenounproject.com/icon/1951204) by AomAm from [the Noun Project](https://thenounproject.com/).

The font used in the logo is [Repo](https://fontlibrary.org/en/font/repo) by Stefan Peev, from [Font Library](https://fontlibrary.org).
