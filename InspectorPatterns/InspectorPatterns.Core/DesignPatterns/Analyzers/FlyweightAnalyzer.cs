using InspectorPatterns.Core.DesignPatterns.Interfaces;
using InspectorPatterns.Core.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Linq;

namespace InspectorPatterns.Core.DesignPatterns.Analyzers
{
    public class FlyweightAnalyzer : IAnalyzer, IFlyweightPattern
    {
        private readonly SyntaxNode _classTree;
        private Location location;

        public FlyweightAnalyzer(SyntaxNodeAnalysisContext context)
        {
            _classTree = context.Node.SyntaxTree.GetRoot();
        }

        public bool Analyze()
        {
            if (HasUniqueState() && HasCacheState())
            {
                location = _classTree.DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault().Identifier.GetLocation();

                return true;
            }

            return false;
        }

        public Location GetLocation()
        {
            throw new NotImplementedException();
        }

        public bool HasUniqueState()
        {
            // var uniqueStateDecleration = _classTree.DescendantNodes().OfType<>().FirstOrDefault().Identifier;

            return false;
        }

        public bool HasCacheState()
        {
            // var cacheStateDecleration = _classTree.DescendantNodes().OfType<>().FirstOrDefault().Identifier;

            return false;
        }
    }
}
