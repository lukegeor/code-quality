using System.Diagnostics.CodeAnalysis;

namespace CodeQuality.Analyzers;

/// <summary>
/// Provides diagnostic categories for CodeQuality analyzers.
/// </summary>
[ExcludeFromCodeCoverage]
public static class DiagnosticCategories
{
    /// <summary>
    /// Category for rules enhancing code reliability and preventing runtime exceptions.
    /// </summary>
    public const string Reliability = "Reliability";

    /// <summary>
    /// Category for rules enforcing architectural and design conventions.
    /// </summary>
    public const string Design = "Design";

    /// <summary>
    /// Category for rules enforcing code readability and cleanliness.
    /// </summary>
    public const string Readability = "Readability";
}
