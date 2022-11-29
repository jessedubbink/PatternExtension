using InspectorPatterns.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace InspectorPatterns
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AnalyzerAnalyzer : DiagnosticAnalyzer
    {
        public const string Category = "Usage";

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(MethodAnalyzer.Rule, SingletonAnalyzer.Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.CompilationUnit);
            context.RegisterSyntaxNodeAction(Method, SyntaxKind.CompilationUnit);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            new SingletonAnalyzer(context);
        }

        private void Method(SyntaxNodeAnalysisContext context)
        {
            new MethodAnalyzer(context);
        }
    }
}
