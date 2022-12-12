using InspectorPatterns.Core.DesignPatterns.Interfaces;
using InspectorPatterns.Core.Interfaces;
using Microsoft.CodeAnalysis.Diagnostics;
using System;

namespace InspectorPatterns.Core.DesignPatterns.Analyzers
{
    public class FlyweightAnalyzer : IAnalyzer, IFlyweightPattern
    {
        private readonly SyntaxNodeAnalysisContext _context;

        public FlyweightAnalyzer(SyntaxNodeAnalysisContext context)
        {
            _context = context;
        }

        public bool Analyze()
        {
            throw new NotImplementedException();
        }

        public Location GetLocation()
        {
            throw new NotImplementedException();
        }

        public bool HasUniqueState()
        {
            bool result = false;

            var classTree = _context.Node.SyntaxTree.GetRoot();
            // var uniqueStateDecleration = classTree.DescendantNodes().OfType<>().FirstOrDefault().Identifier;

            return result;
        }

        public bool HasCacheState()
        {
            bool result = false;

            var classTree = _context.Node.SyntaxTree.GetRoot();
            // var cacheStateDecleration = classTree.DescendantNodes().OfType<>().FirstOrDefault().Identifier;

            return result;
        }
    }
}
