# Trailing Comma Analyzer

[![Build status](https://github.com/levimcgomes/TrailingCommaAnalyzer/actions/workflows/main.yml/badge.svg)](https://github.com/levimcgomes/TrailingCommaAnalyzer/actions/workflows/main.yml)

> **This project is a WIP!**

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
- [ ] object and list patterns
