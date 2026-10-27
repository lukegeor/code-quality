using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CodeQuality.Analyzers.Rules.LG0001;

/// <summary>
/// Diagnostic analyzer that detects and discourages the use of the null-forgiving operator ('!').
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class AvoidNullForgivingOperatorAnalyzer : DiagnosticAnalyzer
{
    private const string Title = "Avoid null-forgiving operator";
    private const string MessageFormat = "The null-forgiving operator '!' should be avoided; handle nullability structurally instead";
    private const string Description = "The null-forgiving operator (!) suppresses compiler nullability warnings instead of addressing them structurally. Avoid its usage in favor of explicit null handling, pattern matching, or non-null initialization.";
    private const string HelpLinkUri = "https://github.com/lukegeor/code-quality/blob/main/docs/rules/LG0001.md";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticIds.AvoidNullForgivingOperator,
        Title,
        MessageFormat,
        DiagnosticCategories.Reliability,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: Description,
        helpLinkUri: HelpLinkUri);

    /// <summary>
    /// Initializes a new instance of the <see cref="AvoidNullForgivingOperatorAnalyzer"/> class.
    /// </summary>
    public AvoidNullForgivingOperatorAnalyzer()
    {
    }

    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, SyntaxKind.SuppressNullableWarningExpression);
    }

    private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
    {
        var postfixUnary = (PostfixUnaryExpressionSyntax)context.Node;
        context.ReportDiagnostic(Diagnostic.Create(Rule, postfixUnary.OperatorToken.GetLocation()));
    }
}
