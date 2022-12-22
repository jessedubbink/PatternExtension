using InspectorPatterns.Core;
using InspectorPatterns.Core.DesignPatterns.Analyzers;
using InspectorPatterns.Core.DesignPatterns.Interfaces;
using InspectorPatterns.Core.Interfaces;
using InspectorPatterns.Core.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading;

namespace InspectorPatterns
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SingletonPatternAnalyzer : DiagnosticAnalyzer
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

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            // TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
            // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
            context.RegisterSyntaxNodeAction(AnalyzeFieldNode, SyntaxKind.FieldDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeConstructorNode, SyntaxKind.ConstructorDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeMethodNode, SyntaxKind.MethodDeclaration);
        }

        private bool hasPrivateStaticSelfField;
        private bool hasPrivateConstructor;
        private bool hasGetInstanceSelfMethod;
        private bool isSingleton;

        private readonly DesignPatternAnalyzer analyzer = new DesignPatternAnalyzer();

        private void AnalyzeMethodNode(SyntaxNodeAnalysisContext context)
        {
            analyzer.SetContext(context);
            analyzer.SetAnalyzerStrategy(new SingletonAnalyzer.HasGetInstanceSelfMethod(analyzer.GetContext()));

            if (!analyzer.Analyze())
            {
                hasGetInstanceSelfMethod = false;
                isSingleton = false;

                return;
            }
            
            hasGetInstanceSelfMethod = true;

            if (!isSingleton)
            {
                CheckIfSingleton(context);
            }
        }

        private void AnalyzeConstructorNode(SyntaxNodeAnalysisContext context)
        {
            analyzer.SetContext(context);
            analyzer.SetAnalyzerStrategy(new SingletonAnalyzer.HasPrivateConstructor(analyzer.GetContext()));

            if (!analyzer.Analyze())
            {
                hasGetInstanceSelfMethod = false;
                isSingleton = false;

                return;
            }

            hasPrivateConstructor = true;

            if (!isSingleton)
            {
                CheckIfSingleton(context);
            }
        }

        private void AnalyzeFieldNode(SyntaxNodeAnalysisContext context)
        {
            analyzer.SetContext(context);
            analyzer.SetAnalyzerStrategy(new SingletonAnalyzer.HasPrivateStaticSelfField(analyzer.GetContext()));

            if (!analyzer.Analyze())
            {
                hasGetInstanceSelfMethod = false;
                isSingleton = false;

                return;
            }

            hasPrivateStaticSelfField = true;

            if (!isSingleton)
            {
                CheckIfSingleton(context);
            }
        }

        private void CheckIfSingleton(SyntaxNodeAnalysisContext context)
        {
            if (hasPrivateStaticSelfField && hasPrivateConstructor && hasGetInstanceSelfMethod)
            {
                isSingleton = true;
                Location location = context.Node.GetLocation();// .SyntaxTree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault().Identifier.GetLocation();
                context.ReportDiagnostic(Diagnostic.Create(Rule, location));
            }
        }
    }
}
