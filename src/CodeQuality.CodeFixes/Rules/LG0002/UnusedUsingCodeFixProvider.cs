using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using CodeQuality.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeQuality.CodeFixes.Rules.LG0002;

/// <summary>
/// Provides a code fix to remove unnecessary using directives.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UnusedUsingCodeFixProvider))]
[Shared]
public sealed class UnusedUsingCodeFixProvider : CodeFixProvider
{
    private const string CodeActionTitle = "Remove unnecessary using directive";

    /// <summary>
    /// Initializes a new instance of the <see cref="UnusedUsingCodeFixProvider"/> class.
    /// </summary>
    public UnusedUsingCodeFixProvider()
    {
    }

    /// <inheritdoc />
    public override ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create(DiagnosticIds.RemoveUnnecessaryUsings);

    /// <inheritdoc />
    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    /// <inheritdoc />
    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        RegisterCodeFixes(context, root);
    }

    internal static void RegisterCodeFixes(CodeFixContext context, SyntaxNode? root)
    {
        if (root is null)
        {
            return;
        }

        var diagnostic = context.Diagnostics[0];
        var diagnosticSpan = diagnostic.Location.SourceSpan;
        var node = root.FindNode(diagnosticSpan, getInnermostNodeForTie: true);
        var usingDirective = node.FirstAncestorOrSelf<UsingDirectiveSyntax>();

        if (usingDirective is not null)
        {
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeActionTitle,
                    createChangedDocument: cancellationToken => RemoveUsingAsync(context.Document, root, usingDirective, cancellationToken),
                    equivalenceKey: nameof(UnusedUsingCodeFixProvider)),
                diagnostic);
        }
    }

    internal static Task<Document> RemoveUsingAsync(
        Document document,
        SyntaxNode root,
        UsingDirectiveSyntax usingDirective,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var newRoot = root.RemoveNode(usingDirective, SyntaxRemoveOptions.KeepNoTrivia);
        if (newRoot is { } nonNullRoot)
        {
            return Task.FromResult(document.WithSyntaxRoot(nonNullRoot));
        }

        return Task.FromResult(document);
    }
}
