using InspectorPatterns.Core;
using InspectorPatterns.Core.Analyzers;
using InspectorPatterns.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace InspectorPatterns
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class FlyweightPatternAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "InspectorPatterns";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.FlyweightTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.FlyweightMessage), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.FlyweightDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Naming";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Info, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        private DesignPatternAnalyzer analyzer;

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            if (analyzer == null)
            {
                analyzer = new DesignPatternAnalyzer();
            }

            // TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
            // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
            context.RegisterSyntaxNodeAction(AnalyzeFieldNode, SyntaxKind.FieldDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeMethodNode, SyntaxKind.MethodDeclaration);
        }

        private void AnalyzeFieldNode(SyntaxNodeAnalysisContext context)
        {
            analyzer.SetAnalyzerStrategy(new FlyweightAnalyzer.HasFlyweightCollection(context));

            if (!analyzer.Analyze())
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Rule, context.Node.GetLocation()));
        }

        private void AnalyzeMethodNode(SyntaxNodeAnalysisContext context)
        {
            analyzer.SetAnalyzerStrategy(new FlyweightAnalyzer.HasGetFlyweightMethod(context));

            if (!analyzer.Analyze())
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Rule, context.Node.GetLocation()));
        }
    }
}
