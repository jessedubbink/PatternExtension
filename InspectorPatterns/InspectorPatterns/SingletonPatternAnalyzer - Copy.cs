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
    public class SingletonPatternAnalyzerCopy : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SingletonPatternAnalyzer";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.SingletonTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.SingletonMessage), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.SingletonDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Singleton";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        //private bool hasPrivateStaticSelfField, hasPrivateConstructor, hasGetInstanceSelfMethod;

        private DesignPatternAnalyzer analyzer;// = new DesignPatternAnalyzer();

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            if (analyzer == null)
            {
                analyzer = new DesignPatternAnalyzer();
            }

            // Reset values
            //if (hasPrivateStaticSelfField && hasPrivateConstructor && hasGetInstanceSelfMethod)
            //{
            //    hasPrivateStaticSelfField = false;
            //    hasPrivateConstructor = false;
            //    hasGetInstanceSelfMethod = false;
            //}

            // TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
            // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
            context.RegisterSyntaxNodeAction(AnalyzeFieldNode, SyntaxKind.FieldDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeConstructorNode, SyntaxKind.ConstructorDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeMethodNode, SyntaxKind.MethodDeclaration);
        }

        private void AnalyzeMethodNode(SyntaxNodeAnalysisContext context)
        {
            //analyzer.SetContext(context);
            analyzer.SetAnalyzerStrategy(new SingletonAnalyzer.HasGetInstanceSelfMethod(ContextConverter.Convert(context)));

            if (!analyzer.Analyze())
            {
                //hasGetInstanceSelfMethod = false;

                return;
            }
            
            //hasGetInstanceSelfMethod = true;
            //CheckIfSingleton(context);

            context.ReportDiagnostic(Diagnostic.Create(Rule, context.Node.GetLocation()));
        }

        private void AnalyzeConstructorNode(SyntaxNodeAnalysisContext context)
        {
            //analyzer.SetContext(context);
            analyzer.SetAnalyzerStrategy(new SingletonAnalyzer.HasPrivateConstructor(ContextConverter.Convert(context)));

            if (!analyzer.Analyze())
            {
                //hasPrivateConstructor = false;

                return;
            }

            //hasPrivateConstructor = true;
            //CheckIfSingleton(context);

            context.ReportDiagnostic(Diagnostic.Create(Rule, context.Node.GetLocation()));
        }

        private void AnalyzeFieldNode(SyntaxNodeAnalysisContext context)
        {
            //analyzer.SetContext(context);
            analyzer.SetAnalyzerStrategy(new SingletonAnalyzer.HasPrivateStaticSelfField(ContextConverter.Convert(context)));

            if (!analyzer.Analyze())
            {
                //hasPrivateStaticSelfField = false;

                return;
            }

            //hasPrivateStaticSelfField = true;
            //CheckIfSingleton(context);

            context.ReportDiagnostic(Diagnostic.Create(Rule, context.Node.GetLocation()));
        }

        //private void CheckIfSingleton(SyntaxNodeAnalysisContext context)
        //{
        //    if (hasPrivateStaticSelfField && hasPrivateConstructor && hasGetInstanceSelfMethod)
        //    {
        //        Location location = context.Node.GetLocation();
        //        context.ReportDiagnostic(Diagnostic.Create(Rule, location));
        //    }
        //}
    }
}
