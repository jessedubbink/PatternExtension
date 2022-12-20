using InspectorPatterns.Core.DesignPatterns.Interfaces;
using InspectorPatterns.Core.Interfaces;
using InspectorPatterns.Core.Models;
using Microsoft.CodeAnalysis;
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
