using System.Diagnostics.CodeAnalysis;

namespace CodeQuality.Analyzers;

/// <summary>
/// Provides centralized diagnostic identifiers for all CodeQuality analyzer rules.
/// </summary>
[ExcludeFromCodeCoverage]
public static class DiagnosticIds
{
    /// <summary>
    /// Diagnostic identifier for avoiding the null-forgiving operator ('!').
    /// </summary>
    public const string AvoidNullForgivingOperator = "LG0001";

    /// <summary>
    /// Diagnostic identifier for removing unnecessary using directives.
    /// </summary>
    public const string RemoveUnnecessaryUsings = "LG0002";
}
