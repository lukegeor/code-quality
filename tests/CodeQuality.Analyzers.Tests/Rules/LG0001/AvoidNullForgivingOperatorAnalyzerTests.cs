using System.Threading.Tasks;
using CodeQuality.Analyzers.Rules.LG0001;
using CodeQuality.Analyzers.Tests.Verifiers;
using Xunit;

namespace CodeQuality.Analyzers.Tests.Rules.LG0001;

public sealed class AvoidNullForgivingOperatorAnalyzerTests
{
    [Fact]
    public async Task WhenNoNullForgivingOperator_ReportsNoDiagnostic()
    {
        const string testCode = @"
public class SampleClass
{
    public void Process(string value)
    {
        _ = value.Length;
    }
}";

        await CSharpAnalyzerVerifier<AvoidNullForgivingOperatorAnalyzer>.VerifyAnalyzerAsync(testCode);
    }

    [Fact]
    public async Task WhenNullForgivingOperatorUsed_ReportsDiagnostic()
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

        await CSharpAnalyzerVerifier<AvoidNullForgivingOperatorAnalyzer>.VerifyAnalyzerAsync(testCode);
    }

    [Fact]
    public async Task WhenMultipleNullForgivingOperatorsUsed_ReportsMultipleDiagnostics()
    {
        const string testCode = @"
#nullable enable
public class SampleClass
{
    private string name = default{|LG0001:!|};

    public void Process(string? value)
    {
        _ = value{|LG0001:!|}.Length;
        _ = value{|LG0001:!|}.Trim();
    }
}";

        await CSharpAnalyzerVerifier<AvoidNullForgivingOperatorAnalyzer>.VerifyAnalyzerAsync(testCode);
    }
}
