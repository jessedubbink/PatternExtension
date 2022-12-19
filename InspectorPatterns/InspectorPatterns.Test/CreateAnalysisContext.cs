using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Threading;

namespace InspectorPatterns.Test
{
    public class CreateAnalysisContext
    {
        SyntaxNodeAnalysisContext _context;

        public CreateAnalysisContext(string code)
        {
            var statementSyntax = SyntaxFactory.ParseStatement(code);
            var syntaxNode = SyntaxFactory.NodeOrTokenList(statementSyntax).FirstOrDefault().AsNode();
            _context = new SyntaxNodeAnalysisContext(syntaxNode, null, null, null, null, new CancellationToken());
        }

        public SyntaxNodeAnalysisContext GetContext()
        {
            return _context;
        }
    }
}
