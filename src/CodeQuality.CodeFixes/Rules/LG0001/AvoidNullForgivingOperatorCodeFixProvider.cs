using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using CodeQuality.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeQuality.CodeFixes.Rules.LG0001;

/// <summary>
/// Provides a code fix to remove the null-forgiving operator ('!').
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AvoidNullForgivingOperatorCodeFixProvider))]
[Shared]
public sealed class AvoidNullForgivingOperatorCodeFixProvider : CodeFixProvider
{
    private const string CodeActionTitle = "Remove null-forgiving operator";

    /// <summary>
    /// Initializes a new instance of the <see cref="AvoidNullForgivingOperatorCodeFixProvider"/> class.
    /// </summary>
    public AvoidNullForgivingOperatorCodeFixProvider()
    {
    }

    /// <inheritdoc />
    public override ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create(DiagnosticIds.AvoidNullForgivingOperator);

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
        var token = root.FindToken(diagnosticSpan.Start);

        if (token.Parent is PostfixUnaryExpressionSyntax postfixUnary)
        {
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeActionTitle,
                    createChangedDocument: cancellationToken => RemoveOperatorAsync(context.Document, root, postfixUnary, cancellationToken),
                    equivalenceKey: nameof(AvoidNullForgivingOperatorCodeFixProvider)),
                diagnostic);
        }
    }

    private static Task<Document> RemoveOperatorAsync(
        Document document,
        SyntaxNode root,
        PostfixUnaryExpressionSyntax postfixUnary,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var newOperand = postfixUnary.Operand
            .WithTrailingTrivia(postfixUnary.GetTrailingTrivia());
        var newRoot = root.ReplaceNode(postfixUnary, newOperand);
        return Task.FromResult(document.WithSyntaxRoot(newRoot));
    }
}
