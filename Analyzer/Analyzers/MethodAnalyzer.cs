using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Linq;

namespace Analyzer.Analyzers
{
    public class MethodAnalyzer
    {
        public const string DiagnosticId = "Method";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.MethodTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.MethodMessage), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.MethodDescription), Resources.ResourceManager, typeof(Resources));

        public static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerAnalyzer.Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public MethodAnalyzer(SyntaxNodeAnalysisContext context)
        {
            var classTree = context.Node.SyntaxTree.GetRoot() as CompilationUnitSyntax;
            var methodDeclaration = classTree.DescendantNodes().OfType<MethodDeclarationSyntax>().FirstOrDefault();

            if (methodDeclaration == null)
            {
                return;
            }

            if (!methodDeclaration.Modifiers.Any(SyntaxKind.PrivateKeyword))
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Rule, methodDeclaration.GetLocation()));
        }
    }
}
