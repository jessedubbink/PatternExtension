using InspectorPatterns.Core.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System;

namespace InspectorPatterns.Core.Analyzers
{
    public class FlyweightAnalyzer : IAnalyzer
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

        public bool HasCacheState()
        {
            throw new NotImplementedException();
        }

        public bool HasUniqueState()
        {
            throw new NotImplementedException();
        }
    }
}
