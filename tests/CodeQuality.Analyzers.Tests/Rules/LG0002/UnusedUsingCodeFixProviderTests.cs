using System.Threading;
using System.Threading.Tasks;
using CodeQuality.Analyzers;
using CodeQuality.Analyzers.Rules.LG0002;
using CodeQuality.Analyzers.Tests.Verifiers;
using CodeQuality.CodeFixes.Rules.LG0002;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Xunit;

namespace CodeQuality.Analyzers.Tests.Rules.LG0002;

public sealed class UnusedUsingCodeFixProviderTests
{
    [Fact]
    public async Task WhenUnusedUsingPresent_RemovesUnusedUsing()
    {
        const string testCode = @"[|using System;|]
using System.Collections.Generic;

public class TestClass
{
    public List<int>? Items { get; set; }
}";

        const string fixedCode = @"using System.Collections.Generic;

public class TestClass
{
    public List<int>? Items { get; set; }
}";

        await CSharpCodeFixVerifier<UnusedUsingAnalyzer, UnusedUsingCodeFixProvider>
            .VerifyCodeFixAsync(testCode, fixedCode);
    }

    [Fact]
    public async Task WhenMultipleUnusedUsingsPresent_FixAllRemovesAll()
    {
        const string testCode = @"[|using System;|]
[|using System.IO;|]
using System.Collections.Generic;

public class TestClass
{
    public List<int>? Items { get; set; }
}";

        const string fixedCode = @"using System.Collections.Generic;

public class TestClass
{
    public List<int>? Items { get; set; }
}";

        await CSharpCodeFixVerifier<UnusedUsingAnalyzer, UnusedUsingCodeFixProvider>
            .VerifyCodeFixAsync(testCode, fixedCode);
    }

    [Fact]
    public async Task WhenRootIsNull_RegisterCodeFixesReturnsEarly()
    {
        using var workspace = new AdhocWorkspace();
        var project = workspace.AddProject("TestProj", LanguageNames.CSharp);
        var document = workspace.AddDocument(project.Id, "test.cs", SourceText.From("class TestClass {}"));

        var diagnostic = Diagnostic.Create(
            DiagnosticIds.RemoveUnnecessaryUsings,
            DiagnosticCategories.Readability,
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

        UnusedUsingCodeFixProvider.RegisterCodeFixes(context, root: null);

        Assert.False(registered);
    }

    [Fact]
    public async Task WhenDiagnosticTargetIsNotUsingDirective_RegisterCodeFixesDoesNotRegisterFix()
    {
        var provider = new UnusedUsingCodeFixProvider();
        using var workspace = new AdhocWorkspace();
        var project = workspace.AddProject("TestProj", LanguageNames.CSharp);
        var document = workspace.AddDocument(project.Id, "test.cs", SourceText.From("class TestClass {}"));
        var root = await document.GetSyntaxRootAsync();
        Assert.NotNull(root);

        if (root is { } nonNullRoot)
        {
            var token = nonNullRoot.FindToken(0);
            var diagnostic = Diagnostic.Create(
                DiagnosticIds.RemoveUnnecessaryUsings,
                DiagnosticCategories.Readability,
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

    [Fact]
    public async Task WhenRootIsUsingDirectiveItself_RemoveUsingAsyncReturnsOriginalDocument()
    {
        using var workspace = new AdhocWorkspace();
        var project = workspace.AddProject("TestProj", LanguageNames.CSharp);
        var document = workspace.AddDocument(project.Id, "test.cs", SourceText.From("using System;"));

        var usingSyntax = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System"));
        var resultDoc = await UnusedUsingCodeFixProvider.RemoveUsingAsync(
            document,
            root: usingSyntax,
            usingDirective: usingSyntax,
            CancellationToken.None);

        Assert.Same(document, resultDoc);
    }
}
