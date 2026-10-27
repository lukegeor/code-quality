using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;

namespace CodeQuality.Analyzers.Tests.Verifiers;

/// <summary>
/// A utility class for verifying Roslyn analyzers and code fixes against C# source text.
/// </summary>
/// <typeparam name="TAnalyzer">The type of the diagnostic analyzer under test.</typeparam>
/// <typeparam name="TCodeFix">The type of the code fix provider under test.</typeparam>
public static class CSharpCodeFixVerifier<TAnalyzer, TCodeFix>
    where TAnalyzer : DiagnosticAnalyzer, new()
    where TCodeFix : CodeFixProvider, new()
{
    /// <summary>
    /// Creates a diagnostic result with the specified diagnostic identifier.
    /// </summary>
    /// <param name="diagnosticId">The expected diagnostic identifier.</param>
    /// <returns>A configured <see cref="DiagnosticResult"/>.</returns>
    public static DiagnosticResult Diagnostic(string diagnosticId) =>
        CSharpCodeFixVerifier<TAnalyzer, TCodeFix, DefaultVerifier>.Diagnostic(diagnosticId);

    /// <summary>
    /// Creates a diagnostic result matching the specified descriptor.
    /// </summary>
    /// <param name="descriptor">The diagnostic descriptor.</param>
    /// <returns>A configured <see cref="DiagnosticResult"/>.</returns>
    public static DiagnosticResult Diagnostic(DiagnosticDescriptor descriptor) =>
        CSharpCodeFixVerifier<TAnalyzer, TCodeFix, DefaultVerifier>.Diagnostic(descriptor);

    /// <summary>
    /// Verifies that the specified source code produces the expected fixed code after applying the code fix.
    /// </summary>
    /// <param name="source">The original C# source code containing violations.</param>
    /// <param name="fixedSource">The expected C# source code after applying the fix.</param>
    /// <returns>A task representing the verification operation.</returns>
    public static Task VerifyCodeFixAsync(string source, string fixedSource) =>
        VerifyCodeFixAsync(source, DiagnosticResult.EmptyDiagnosticResults, fixedSource);

    /// <summary>
    /// Verifies that the specified source code produces diagnostics and the expected fixed code.
    /// </summary>
    /// <param name="source">The original C# source code containing violations.</param>
    /// <param name="expected">The expected diagnostics.</param>
    /// <param name="fixedSource">The expected C# source code after applying the fix.</param>
    /// <returns>A task representing the verification operation.</returns>
    public static Task VerifyCodeFixAsync(string source, DiagnosticResult[] expected, string fixedSource)
    {
        var test = new CSharpCodeFixTest<TAnalyzer, TCodeFix, DefaultVerifier>
        {
            TestCode = source,
            FixedCode = fixedSource,
        };

        test.ExpectedDiagnostics.AddRange(expected);
        return test.RunAsync(CancellationToken.None);
    }
}
