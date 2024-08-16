# Trailing Comma Analyzer

[![Build status](https://github.com/levimcgomes/TrailingCommaAnalyzer/actions/workflows/main.yml/badge.svg)](https://github.com/levimcgomes/TrailingCommaAnalyzer/actions/workflows/main.yml)

This is a C# code analyzer, based on the .NET Compiler SDK, which enforces the usage of trailing commas where they're allowed.

## Motivation

[Trailing commas are good](https://devblogs.microsoft.com/oldnewthing/20240209-00/?p=109379) - it's hard to argue. .NET's default analyzers don't enforce them. This analyzer fills that gap.

## Features

Report and fix missing trailing commas in:

- [x] object initializers
- [x] array initializers
- [x] collection initializers
- [x] anonymous objects
- [x] enum declarations
- [x] switch expressions
- [x] with initializers
- [x] object and list patterns

## Usage

Install using one of the methods described in [the package's NuGet page](https://www.nuget.org/packages/levimcgomes.TrailingCommaAnalyzer/) and enjoy!

## Configuration

The analyzer can be configured with an `.editorconfig` file. The available option are:
 
- `dotnet_diagnostic.TCA001.trailing_comma_style`
- `dotnet_diagnostic.TCA001.severity`

### `trailing_comma_style`
This option determines where trailing commas should be present. The allowed values are:

- **`always`**: a trailing comma should always be used
- **`never`**: a trailing comma should never be used (this effectively disables the analyzer).
- **`end_of_line`**: a trailing comma should only be used if it is at a line ending.

The default value is `end_of_line`, and it will be assumed if an invalid value is specified.

### `severity`
Please refer to [the official documentation](https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/configuration-options#severity-level) for allowed values and their effects. The default value is `warning`