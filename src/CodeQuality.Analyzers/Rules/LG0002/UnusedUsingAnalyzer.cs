using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CodeQuality.Analyzers.Rules.LG0002;

/// <summary>
/// Diagnostic analyzer that detects and reports unnecessary using directives.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UnusedUsingAnalyzer : DiagnosticAnalyzer
{
    private const string Title = "Remove unnecessary using directive";
    private const string MessageFormat = "Using directive '{0}' is unnecessary";
    private const string Description = "Using directives that are not referenced in the file add unnecessary noise and should be removed.";
    private const string HelpLinkUri = "https://github.com/lukegeor/code-quality/blob/main/docs/rules/LG0002.md";
    private const string CompilerUnnecessaryUsingId = "CS8019";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticIds.RemoveUnnecessaryUsings,
        Title,
        MessageFormat,
        DiagnosticCategories.Readability,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: Description,
        helpLinkUri: HelpLinkUri);

    /// <summary>
    /// Initializes a new instance of the <see cref="UnusedUsingAnalyzer"/> class.
    /// </summary>
    public UnusedUsingAnalyzer()
    {
    }

    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSemanticModelAction(AnalyzeSemanticModel);
    }

    private static void AnalyzeSemanticModel(SemanticModelAnalysisContext context)
    {
        var diagnostics = context.SemanticModel.GetDiagnostics(cancellationToken: context.CancellationToken);
        for (var i = 0; i < diagnostics.Length; i++)
        {
            var diagnostic = diagnostics[i];
            if (diagnostic.Id == CompilerUnnecessaryUsingId)
            {
                var root = context.SemanticModel.SyntaxTree.GetRoot(context.CancellationToken);
                var node = root.FindNode(diagnostic.Location.SourceSpan);
                context.ReportDiagnostic(Diagnostic.Create(Rule, diagnostic.Location, node.ToString()));
            }
        }
    }
}
