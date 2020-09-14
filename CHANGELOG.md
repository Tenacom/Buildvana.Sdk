# Changelog

All notable changes to the Buildvana SDK will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## Unreleased changes

### Added

- **[potentially breaking]** A unit test project is now recognized as such, by convention, if its name ends with `.Tests`.  
  To opt out of this convention, explicitly set `IsTestProject` to `true` or `false`.

### Updated

- Using `StyleCop.Analyzers` version 1.2.0-beta.205

## [1.0.0-alpha.2](https://github.com/Buildvana/Buildvana.Sdk/releases/tag/1.0.0-alpha.2) (2020-09-13)

### Added

- Warning CA1707 (Identifiers should not contain underscores) is now suppressed by default in test projects. You can control this feature via the `AllowUnderscoresInMemberNames` property.

## [1.0.0-alpha.1](https://github.com/Buildvana/Buildvana.Sdk/releases/tag/1.0.0-alpha.1) (2020-09-12)

Initial release.
