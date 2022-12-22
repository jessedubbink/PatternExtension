using InspectorPatterns.Core.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System;

namespace InspectorPatterns.Core
{
    public class DesignPatternAnalyzer
    {
        private IAnalyzer _analyzer;
        private SyntaxNode _context;

        public void SetAnalyzerStrategy(IAnalyzer analyzer)
        {
            _analyzer = analyzer;
        }

        private void ConvertContext(SyntaxNodeAnalysisContext context)
        {
            _context = context.Node.SyntaxTree.GetRoot();
        }

        public void SetContext(SyntaxNodeAnalysisContext context)
        {
            ConvertContext(context);
        }

        public SyntaxNode GetContext()
        {
            return _context;
        }

        public bool Analyze()
        {
            if (_analyzer == null)
            {
                return false;
            }

            var result = _analyzer.Analyze();

            return result;
        }
    }
}
