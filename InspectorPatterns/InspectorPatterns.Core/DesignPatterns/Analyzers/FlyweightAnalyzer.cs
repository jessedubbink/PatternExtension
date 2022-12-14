using InspectorPatterns.Core.DesignPatterns.Interfaces;
using InspectorPatterns.Core.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;

namespace InspectorPatterns.Core.DesignPatterns.Analyzers
{
    public class FlyweightAnalyzer : IAnalyzer, IFlyweightPattern
    {
        private readonly SyntaxNodeAnalysisContext _context;
        private readonly SyntaxNode _classTree;
        private Location location;

        private List<ICollection> collections = new List<ICollection>();

        public FlyweightAnalyzer(SyntaxNodeAnalysisContext context)
        {
            _context = context;
            _classTree = context.Node.SyntaxTree.GetRoot();
        }

        public bool Analyze()
        {
            if (hasCollectionOfObjects() && hasGetFlyweightMethod())
            {
                location = _classTree.DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault().Identifier.GetLocation();
                return true;
            }

            return false;
        }

        public Location GetLocation()
        {
            return location;
        }

        public bool hasCollectionOfObjects()
        {
            List<object> objectCollection = new List<object>();
            var variableDeclarations = _classTree.DescendantNodes().OfType<VariableDeclarationSyntax>();

            foreach (var item in variableDeclarations)
            {
                //var test = _context.SemanticModel.GetSymbolInfo(item.Declaration.Type);

                string typeName = _context.SemanticModel.GetTypeInfo(item).Type.Name;


                //if (variable.Type.GetType().GetInterface(nameof(ICollection)) == null)
                //{
                //    continue;
                //}

                //collections.Add(objectCollection);
            }

            if (collections.Count > 0)
            {
                return true;
            }

            return false;
        }

        public bool hasGetFlyweightMethod()
        {
            if (collections == null)
            {

            }

            if (collections.Count == 0)
            {
                return false;
            }

            var methodDeclarations = _classTree.DescendantNodes().OfType<MethodDeclarationSyntax>();

            if (!methodDeclarations.Any())
            {
                return false;
            }

            foreach (MethodDeclarationSyntax method in methodDeclarations) // N
            {
                if (!method.Modifiers.Any(SyntaxKind.PublicKeyword))
                {
                    continue;
                }

                if (method.ReturnType == null)
                {
                    continue;
                }

                foreach (ICollection collection in collections) // N^2, we should seperate this from the parent loop
                {
                    if (collection.GetType() != method.ReturnType.GetType())
                    {
                        continue;
                    }

                    var ifStatements = method.DescendantNodes().OfType<IfStatementSyntax>().FirstOrDefault();


                    return true;
                }
            }

            return false;
        }
    }
}
