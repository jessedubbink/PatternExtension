using InspectorPatterns.Core.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace InspectorPatterns.Core
{
    public class DesignPatternAnalyzer
    {
        private IAnalyzer _analyzer;
        private SyntaxNode _context;

        public Location Location { get; set; }
        //public Results Results { get; set; }

        public DesignPatternAnalyzer(SyntaxNodeAnalysisContext context)
        {
            ConvertContext(context);
        }

        private void ConvertContext(SyntaxNodeAnalysisContext context)
        {
            _context = context.Node.SyntaxTree.GetRoot();
        }

        public void SetAnalyzerStrategy(IAnalyzer analyzer)
        {
            _analyzer = analyzer;
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

            if (result)
            {
                Location = _analyzer.GetLocation();
            }

            return result;
        }
    }
}
