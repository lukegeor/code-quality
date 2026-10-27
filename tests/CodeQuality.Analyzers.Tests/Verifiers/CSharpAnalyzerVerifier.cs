using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;

namespace CodeQuality.Analyzers.Tests.Verifiers;

/// <summary>
/// A utility class for verifying Roslyn analyzers against C# source text.
/// </summary>
/// <typeparam name="TAnalyzer">The type of the diagnostic analyzer under test.</typeparam>
public static class CSharpAnalyzerVerifier<TAnalyzer>
    where TAnalyzer : DiagnosticAnalyzer, new()
{
    /// <summary>
    /// Creates a diagnostic result with the specified diagnostic identifier.
    /// </summary>
    /// <param name="diagnosticId">The expected diagnostic identifier.</param>
    /// <returns>A configured <see cref="DiagnosticResult"/>.</returns>
    public static DiagnosticResult Diagnostic(string diagnosticId) =>
        CSharpAnalyzerVerifier<TAnalyzer, DefaultVerifier>.Diagnostic(diagnosticId);

    /// <summary>
    /// Creates a diagnostic result matching the specified descriptor.
    /// </summary>
    /// <param name="descriptor">The diagnostic descriptor.</param>
    /// <returns>A configured <see cref="DiagnosticResult"/>.</returns>
    public static DiagnosticResult Diagnostic(DiagnosticDescriptor descriptor) =>
        CSharpAnalyzerVerifier<TAnalyzer, DefaultVerifier>.Diagnostic(descriptor);

    /// <summary>
    /// Verifies that the specified C# source text produces the expected diagnostics.
    /// </summary>
    /// <param name="source">The C# source code.</param>
    /// <param name="expected">The expected diagnostics.</param>
    /// <returns>A task representing the verification operation.</returns>
    public static Task VerifyAnalyzerAsync(string source, params DiagnosticResult[] expected)
    {
        var test = new CSharpAnalyzerTest<TAnalyzer, DefaultVerifier>
        {
            TestCode = source,
        };

        test.ExpectedDiagnostics.AddRange(expected);
        return test.RunAsync(CancellationToken.None);
    }
}
