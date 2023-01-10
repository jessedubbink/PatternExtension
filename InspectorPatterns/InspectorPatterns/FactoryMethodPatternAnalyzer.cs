using InspectorPatterns.Core;
using InspectorPatterns.Core.Analyzers;
using InspectorPatterns.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace InspectorPatterns
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class FactoryMethodPatternAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "FactoryMethodPatternAnalyzer";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.FactoryMethodTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.FactoryMethodMessage), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.FactoryMethodDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Creational design pattern";

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
            context.RegisterSyntaxNodeAction(AnalyzeNode_For_IsProductInterface, SyntaxKind.InterfaceDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeNode_For_IsAbstractFactoryMethod, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeNode_For_OverridesAbstractFactoryMethod, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeNode_For_IsConcreteProduct, SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeNode_For_DeclarationWithFactoryMethod, SyntaxKind.LocalDeclarationStatement);
            // TODO:
            //context.RegisterSyntaxNodeAction(AnalyzeNode_For_ClientCodeThatUsesFactoryMethod, SyntaxKind.InvocationExpression);
        }

        private void AnalyzeNode_For_DeclarationWithFactoryMethod(SyntaxNodeAnalysisContext context)
        {
            analyzer.SetAnalyzerStrategy(new FactoryMethodAnalyzer.DeclarationWithFactoryMethod(context));

            if (!analyzer.Analyze())
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Rule, context.Node.GetLocation()));
        }

        private void AnalyzeNode_For_IsConcreteProduct(SyntaxNodeAnalysisContext context)
        {
            analyzer.SetAnalyzerStrategy(new FactoryMethodAnalyzer.IsConcreteProduct(context));

            if (!analyzer.Analyze())
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Rule, context.Node.GetLocation()));
        }

        private void AnalyzeNode_For_IsProductInterface(SyntaxNodeAnalysisContext context)
        {
            analyzer.SetAnalyzerStrategy(new FactoryMethodAnalyzer.IsProductInterface(context));

            if (!analyzer.Analyze())
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Rule, context.Node.GetLocation()));
        }
        
        private void AnalyzeNode_For_OverridesAbstractFactoryMethod(SyntaxNodeAnalysisContext context)
        {
            analyzer.SetAnalyzerStrategy(new FactoryMethodAnalyzer.OverridesAbstractFactoryMethod(context));

            if (!analyzer.Analyze())
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Rule, context.Node.GetLocation()));
        }

        private void AnalyzeNode_For_IsAbstractFactoryMethod(SyntaxNodeAnalysisContext context)
        {
            analyzer.SetAnalyzerStrategy(new FactoryMethodAnalyzer.IsAbstractFactoryMethod(context));

            if (!analyzer.Analyze())
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Rule, context.Node.GetLocation()));
        }

        private void AnalyzeNode_For_ClientCodeThatUsesFactoryMethod(SyntaxNodeAnalysisContext obj)
        {
            //throw new NotImplementedException();
        }
    }
}
