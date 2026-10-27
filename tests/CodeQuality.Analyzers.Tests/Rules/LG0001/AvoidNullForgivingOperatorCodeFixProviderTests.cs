using System.Threading;
using System.Threading.Tasks;
using CodeQuality.Analyzers;
using CodeQuality.Analyzers.Rules.LG0001;
using CodeQuality.Analyzers.Tests.Verifiers;
using CodeQuality.CodeFixes.Rules.LG0001;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Text;
using Xunit;

namespace CodeQuality.Analyzers.Tests.Rules.LG0001;

public sealed class AvoidNullForgivingOperatorCodeFixProviderTests
{
    [Fact]
    public async Task WhenNullForgivingOperatorUsed_RemovesOperator()
    {
        const string testCode = @"
#nullable enable
public class SampleClass
{
    public void Process(string? value)
    {
        _ = value{|LG0001:!|}.Length;
    }
}";

        const string fixedCode = @"
#nullable enable
public class SampleClass
{
    public void Process(string? value)
    {
        _ = value.Length;
    }
}";

        await CSharpCodeFixVerifier<AvoidNullForgivingOperatorAnalyzer, AvoidNullForgivingOperatorCodeFixProvider>
            .VerifyCodeFixAsync(testCode, fixedCode);
    }

    [Fact]
    public async Task WhenMultipleNullForgivingOperatorsUsed_FixAllRemovesAllOperators()
    {
        const string testCode = @"
#nullable enable
public class SampleClass
{
    public void Process(string? value)
    {
        _ = value{|LG0001:!|}.Length;
        _ = value{|LG0001:!|}.Trim();
    }
}";

        const string fixedCode = @"
#nullable enable
public class SampleClass
{
    public void Process(string? value)
    {
        _ = value.Length;
        _ = value.Trim();
    }
}";

        await CSharpCodeFixVerifier<AvoidNullForgivingOperatorAnalyzer, AvoidNullForgivingOperatorCodeFixProvider>
            .VerifyCodeFixAsync(testCode, fixedCode);
    }

    [Fact]
    public async Task WhenRootIsNull_RegisterCodeFixesReturnsEarly()
    {
        using var workspace = new AdhocWorkspace();
        var project = workspace.AddProject("TestProj", LanguageNames.CSharp);
        var document = workspace.AddDocument(project.Id, "test.cs", SourceText.From("class TestClass {}"));

        var diagnostic = Diagnostic.Create(
            DiagnosticIds.AvoidNullForgivingOperator,
            DiagnosticCategories.Reliability,
            "Test message",
            DiagnosticSeverity.Warning,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            warningLevel: 1,
            location: Location.None);

        var registered = false;
        var context = new CodeFixContext(
            document,
            diagnostic,
            (action, diag) => registered = true,
            CancellationToken.None);

        AvoidNullForgivingOperatorCodeFixProvider.RegisterCodeFixes(context, root: null);

        Assert.False(registered);
    }

    [Fact]
    public async Task WhenDiagnosticTargetIsNotPostfixUnary_RegisterCodeFixesAsyncDoesNotRegisterFix()
    {
        var provider = new AvoidNullForgivingOperatorCodeFixProvider();
        using var workspace = new AdhocWorkspace();
        var project = workspace.AddProject("TestProj", LanguageNames.CSharp);
        var document = workspace.AddDocument(project.Id, "test.cs", SourceText.From("class TestClass {}"));
        var root = await document.GetSyntaxRootAsync();
        Assert.NotNull(root);

        if (root is { } nonNullRoot)
        {
            var token = nonNullRoot.FindToken(0);
            var diagnostic = Diagnostic.Create(
                DiagnosticIds.AvoidNullForgivingOperator,
                DiagnosticCategories.Reliability,
                "Test message",
                DiagnosticSeverity.Warning,
                DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                warningLevel: 1,
                location: token.GetLocation());

            var registered = false;
            var context = new CodeFixContext(
                document,
                diagnostic,
                (action, diag) => registered = true,
                CancellationToken.None);

            await provider.RegisterCodeFixesAsync(context);

            Assert.False(registered);
        }
    }
}
