# CodeQuality

Custom Roslyn analyzer rules and code fixes to enforce code quality, reliability, and architectural standards in C# and .NET projects.

## Included Analyzer Rules

| Rule ID | Title | Category | Severity | Code Fix |
| --- | --- | --- | --- | --- |
| [`LG0001`](docs/rules/LG0001.md) | Avoid null-forgiving operator (`!`) | Reliability | Warning | Yes |
| [`LG0002`](docs/rules/LG0002.md) | Remove unnecessary using directive | Readability | Warning | Yes |

## Quick Start

### Consuming the Package

Add a reference to the `CodeQuality` package in your `.csproj`:

```xml
<ItemGroup>
  <PackageReference Include="CodeQuality" Version="1.0.0">
    <PrivateAssets>all</PrivateAssets>
    <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
  </PackageReference>
</ItemGroup>
```

Analyzers run automatically during build (`dotnet build`) and live in IDEs (Visual Studio, VS Code, JetBrains Rider).

## Repository Architecture

- **`src/CodeQuality.Analyzers`**: Roslyn `DiagnosticAnalyzer` implementations targeting `netstandard2.0` for compatibility with `csc` and all IDE hosts.
- **`src/CodeQuality.CodeFixes`**: Roslyn `CodeFixProvider` implementations providing IDE quick-fixes and refactorings.
- **`src/CodeQuality.Package`**: NuGet packaging project bundling analyzer and code fix binaries into `analyzers/dotnet/cs/`.
- **`tests/CodeQuality.Analyzers.Tests`**: Unit tests verifying analyzer diagnostics and code transformations with 100% test coverage.

For developer guidelines and how to implement new analyzer rules, see [docs/architecture.md](docs/architecture.md).
For instructions on versioning and publishing releases, see [docs/publishing.md](docs/publishing.md).

## Development Commands

```bash
# Build the solution
dotnet build CodeQuality.slnx

# Run tests and collect code coverage
dotnet test CodeQuality.slnx --collect:"XPlat Code Coverage"

# Build Release NuGet package
dotnet pack CodeQuality.slnx -c Release
```

## License

[MIT](LICENSE)
