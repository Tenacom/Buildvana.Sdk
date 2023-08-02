# ![Buildvana SDK](graphics/Readme.png)

[![License](https://badgen.net/badge/license/MIT/blue)](https://github.com/Tenacom/Buildvana.Sdk/blob/main/LICENSE)
[![Latest release](https://badgen.net/github/release/Tenacom/Buildvana.Sdk?label=latest)](https://github.com/Tenacom/Buildvana.Sdk/releases)
[![Latest stable release](https://badgen.net/github/release/Tenacom/Buildvana.Sdk/stable?label=stable)](https://github.com/Tenacom/Buildvana.Sdk/releases)
[![Changelog](https://badgen.net/badge/changelog/Keep%20a%20Changelog%20v1.0.0/orange)](https://github.com/Tenacom/Buildvana.Sdk/blob/main/CHANGELOG.md)

[![Build, test, and pack](https://github.com/Tenacom/Buildvana.Sdk/actions/workflows/build-test-pack.yml/badge.svg)](https://github.com/Tenacom/Buildvana.Sdk/actions/workflows/build-test-pack.yml)
[![CodeQL](https://github.com/Tenacom/Buildvana.Sdk/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/Tenacom/Buildvana.Sdk/actions/workflows/codeql-analysis.yml)

![Repobeats analytics image](https://repobeats.axiom.co/api/embed/733fff6e0c96c981b6229b450fdf4df3e1b4e584.svg "Repobeats analytics image")

| Latest packages | NuGet | Feedz.io |
|-----------------|-------|----------|
| Buildvana.Sdk | [![Buildvana.Sdk @ NuGet](https://badgen.net/nuget/v/Buildvana.Sdk?icon=nuget&label=)](https://nuget.org/packages/Buildvana.Sdk) | ![Buildvana.Sdk @ Feedz](https://img.shields.io/feedz/vpre/tenacom/preview/Buildvana.Sdk?label=&color=orange) |

**You can get preview packages of Buildvana SDK from our preview feed on [Feedz.io](https://feedz.io). The NuGet v3 URL is `https://f.feedz.io/tenacom/preview/nuget/index.json`.**

---

- [At a glance](#at-a-glance)
  - [Benefits](#benefits)
  - [Compatibility](#compatibility)
    - [Project types](#project-types)
    - [Programming languages](#programming-languages)
    - [Git servers](#git-servers)
    - [Toolchain](#toolchain)
- [Quick start](#quick-start)
- [Contributing](#contributing)
- [Contributors](#contributors)
- [Proudly built using Buildvana SDK](#proudly-built-using-buildvana-sdk)

---

## At a glance

Buildvana SDK is an opinionated, best-practices-based, CI-friendly, VS-friendly, MSBuild-powered build system for .NET projects.

> **DISCLAIMER:** Buildvana SDK is still a work in progress.  
Your mileage may vary, if you break your build you own both pieces, and so on and so forth.  
However, Buildvana SDK has already been used successfully in production, for both business and open source projects.

### Benefits

- Helps you keep your project files clean and concise - even better than "plain" MSBuild SDKs
- Single source of truth for assembly versions (via [`Nerdbank.GitVersioning`(https://github.com/dotnet/Nerdbank.GitVersioning)])
- Single source of truth for package licenses and copyright notices
- More auto-generated assembly information (`ClsCompliant`, `COMVisible`)
- Automatic configuration of commonly-used code analyzers
- ...

### Compatibility

#### Project types

- :heart: Multi-platform / Cross-platform projects
- :heart: Libraries
- :heart: Console apps
- :heart: Windows Forms
- :heart: ASP.NET
- :heart: Projects using `Microsoft.Build.NoTargets` SDK
- :heart: [Avalonia UI](https://avaloniaui.net) (still experimenting, but no problems so far)
- :question: WPF (testers welcome)
- :question: [UNO Platform](https://platform.uno) (testers welcome)
- :question: .NET MAUI (testers welcome)
- :thumbsdown: Legacy (non-SDK) projects

#### Programming languages

- :heart: C#
- :heart: Visual Basic
- :yellow_heart: F# (some features disabled)
- :thumbsdown: other languages

#### Git servers

- :heart: GitHub / GitHub Enterprise
- :yellow_heart: All others (no automatic SourceLink configuration)

#### Toolchain

- :heart: MSBuild v17.6 or newer (`msbuild`) running under .NET Framework 4.7.2 or newer / .NET 7.0 or newer
- :heart: Visual Studio 2022 v17.6 or newer (building from IDE)
- :heart: .NET SDK 7.0.306 or newer (`dotnet build`, `dotnet msbuild`, etc.)
- :question: Visual Studio for Mac (_should_ work, but we need confirmation by someone with a Mac)

## Quick start

**TODO**

## Contributing

_Of course_ we accept contributions! :smiley: Just take a look at our [Code of Conduct](https://github.com/Tenacom/.github/blob/main/CODE_OF_CONDUCT.md) and [Contributors guide](https://github.com/Tenacom/.github/blob/main/CONTRIBUTING.md), create your fork, and let's party! :tada:

## Contributors

<!-- ALL-CONTRIBUTORS-LIST:START - Do not remove or modify this section -->
<!-- prettier-ignore-start -->
<!-- markdownlint-disable -->
<table>
  <tbody>
    <tr>
      <td align="center"><a href="https://github.com/rdeago"><img src="https://avatars.githubusercontent.com/u/139223?v=4" width="100px;" alt=""/><br /><sub><b>Riccardo De Agostini</b></sub></a></td>
    </tr>
  </tbody>
  <tfoot>
    <tr>
      <td align="center" size="13px" colspan="7">
        <img src="https://raw.githubusercontent.com/all-contributors/all-contributors-cli/1b8533af435da9854653492b1327a23a4dbd0a10/assets/logo-small.svg">
          <a href="https://all-contributors.js.org/docs/en/bot/usage">Add your contributions</a>
        </img>
      </td>
    </tr>
  </tfoot>
</table>

<!-- markdownlint-restore -->
<!-- prettier-ignore-end -->

<!-- ALL-CONTRIBUTORS-LIST:END -->

## Proudly built using Buildvana SDK

- [PolyKit](https://github.com/Buildvana/PolyKit) is the last polyfill library you'll ever need for your C# projects.
- [L.o.U.I.S.](https://github.com/Tenacom/Louis) is a general-purpose library, providing commonly-used types, suitable for multi-platform libraries and applications.
- [Cecil.XmlDocNames](https://github.com/Tenacom/Cecil.XmlDocNames) is a tiny library that generates XML-documentation-compliant names for [Mono.Cecil](https://github.com/jbevain/cecil) objects.
- [ReSharper.ExportAnnotations](https://github.com/Tenacom/ReSharper.ExportAnnotations) lets you distribute ReSharper code annotations in XML format along with your libraries, without keeping a transient dependency on the [JetBrains.Annotations](https://www.nuget.org/packages/JetBrains.Annotations) package.
- Practically every one of [Tenacom](https://github.com/Tenacom)'s private projects since the first preview of Buildvana SDK: almost 50 libraries, a bunch of console apps, some WinForms apps, even an Avalonia UI app (with more coming soon).
