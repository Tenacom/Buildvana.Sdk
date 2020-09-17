# Changelog

All notable changes to the Buildvana SDK will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## Unreleased changes

### New features

### Changes to existing features

### Bugs fixed in this release

### Known problems introduced by this release

## [1.0.0-alpha.4](https://github.com/Buildvana/Buildvana.Sdk/releases/tag/1.0.0-alpha.4) (2020-09-14)

### Changes to existing features
- #30 The LiteralAssemblyAttributes module now works as expected.

## [1.0.0-alpha.3](https://github.com/Buildvana/Buildvana.Sdk/releases/tag/1.0.0-alpha.3) (2020-09-14)

### New features
- **[potentially breaking]** #26 A unit test project is now recognized as such, by convention, if its name ends with `.Tests`.  
  To opt out of this convention, explicitly set `IsTestProject` to `true` or `false`.

### Changes to existing features
- Using `StyleCop.Analyzers` version 1.2.0-beta.205

## [1.0.0-alpha.2](https://github.com/Buildvana/Buildvana.Sdk/releases/tag/1.0.0-alpha.2) (2020-09-13)

### New features
- #22 Warning CA1707 (Identifiers should not contain underscores) is now suppressed by default in test projects. You can control this feature via the `AllowUnderscoresInMemberNames` property.

## [1.0.0-alpha.1](https://github.com/Buildvana/Buildvana.Sdk/releases/tag/1.0.0-alpha.1) (2020-09-12)

Initial release.
