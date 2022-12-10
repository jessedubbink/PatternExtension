using InspectorPatterns.Core.DesignPatterns.Interfaces;
using InspectorPatterns.Core.Interfaces;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Text;

namespace InspectorPatterns.Core.DesignPatterns.Analyzers
{
    public class SingletonAnalyzer : IAnalyzer, ISingletonPattern
    {
        private readonly SyntaxNodeAnalysisContext _context;

        public SingletonAnalyzer(SyntaxNodeAnalysisContext context)
        {
            _context = context;
        }

        public void Analyze()
        {
            throw new NotImplementedException();
        }

        public bool HasGetInstance()
        {
            throw new NotImplementedException();
        }

        public bool HasPrivateConstructor()
        {
            throw new NotImplementedException();
        }

        public bool HasStaticSelf()
        {
            throw new NotImplementedException();
        }
    }
}
