using System.Threading.Tasks;
using CodeQuality.Analyzers.Rules.LG0002;
using CodeQuality.Analyzers.Tests.Verifiers;
using Xunit;

namespace CodeQuality.Analyzers.Tests.Rules.LG0002;

public sealed class UnusedUsingAnalyzerTests
{
    [Fact]
    public async Task WhenAllUsingsUsed_ReportsNoDiagnostic()
    {
        const string testCode = @"
using System.Collections.Generic;

public class TestClass
{
    public List<int>? Items { get; set; }
}";

        await CSharpAnalyzerVerifier<UnusedUsingAnalyzer>.VerifyAnalyzerAsync(testCode);
    }

    [Fact]
    public async Task WhenUnusedUsingPresent_ReportsDiagnostic()
    {
        const string testCode = @"
[|using System;|]
using System.Collections.Generic;

public class TestClass
{
    public List<int>? Items { get; set; }
}";

        await CSharpAnalyzerVerifier<UnusedUsingAnalyzer>.VerifyAnalyzerAsync(testCode);
    }

    [Fact]
    public async Task WhenMultipleUnusedUsingsPresent_ReportsMultipleDiagnostics()
    {
        const string testCode = @"
[|using System;|]
[|using System.IO;|]
using System.Collections.Generic;

public class TestClass
{
    public List<int>? Items { get; set; }
}";

        await CSharpAnalyzerVerifier<UnusedUsingAnalyzer>.VerifyAnalyzerAsync(testCode);
    }

    [Fact]
    public async Task WhenNonUnusedUsingDiagnosticPresent_IgnoresOtherDiagnostics()
    {
        const string testCode = @"
#pragma warning disable CS0219
public class TestClass
{
    public void Method()
    {
        int unused = 42;
    }
}";

        await CSharpAnalyzerVerifier<UnusedUsingAnalyzer>.VerifyAnalyzerAsync(testCode);
    }
}
