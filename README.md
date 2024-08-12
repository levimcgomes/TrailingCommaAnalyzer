# Trailing Comma Analyzer

[![Build status](https://github.com/levimcgomes/TrailingCommaAnalyzer/actions/workflows/main.yml/badge.svg)](https://github.com/levimcgomes/TrailingCommaAnalyzer/actions/workflows/main.yml)

This is a C# code analyzer, based on the .NET Compiler SDK, which enforces the usage of trailing commas where they're allowed.

## Motivation

[Trailing commas are good](https://devblogs.microsoft.com/oldnewthing/20240209-00/?p=109379) - it's hard to argue. .NET's default analyzers don't enforce them. This analyzer fills that gap.

## Features

### Implemented

### Planned

Report missing trailing commas in:

- object initializers
- array initializers
- collection initializers
- enums
- switch expressions

Fix missing commas.
