using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Linq;

namespace Analyzer.Analyzers
{
    public class SingletonAnalyzer
    {
        public const string DiagnosticId = "Singleton";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.SingletonTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.SingletonMessage), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.SingletonDescription), Resources.ResourceManager, typeof(Resources));

        public static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerAnalyzer.Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public SingletonAnalyzer(SyntaxNodeAnalysisContext context)
        {
            var classTree = context.Node.SyntaxTree.GetRoot() as CompilationUnitSyntax;
            var constructorDeclaration = classTree.DescendantNodes().OfType<ConstructorDeclarationSyntax>().FirstOrDefault();
            var variableDeclaration = classTree.DescendantNodes().OfType<FieldDeclarationSyntax>().FirstOrDefault();

            if (constructorDeclaration == null || constructorDeclaration == null)
            {
                return;
            }

            if (!constructorDeclaration.Modifiers.Any(SyntaxKind.PrivateKeyword))
            {
                return;
            }

            if (variableDeclaration != null && !variableDeclaration.Modifiers.Any(SyntaxKind.PrivateKeyword))
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Rule, constructorDeclaration.GetLocation()));
        }
    }
}
