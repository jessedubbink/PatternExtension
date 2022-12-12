using InspectorPatterns.Core.DesignPatterns.Interfaces;
using InspectorPatterns.Core.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InspectorPatterns.Core.DesignPatterns.Analyzers
{
    public class SingletonAnalyzer : IAnalyzer, ISingletonPattern
    {
        private readonly SyntaxNodeAnalysisContext _context;
        private readonly SyntaxNode _classTree;
        private Location location;

        public SingletonAnalyzer(SyntaxNodeAnalysisContext context)
        {
            _context = context;
            _classTree = _context.Node.SyntaxTree.GetRoot();
        }

        public bool Analyze()
        {
            if (HasGetInstanceSelfMethod() && HasPrivateConstructor() && HasPrivateStaticSelfField())
            {
                location = _classTree.DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault().Identifier.GetLocation();

                return true;
            }

            return false;
        }

        public bool HasGetInstanceSelfMethod()
        {
            var methodDeclarations = _classTree.DescendantNodes().OfType<MethodDeclarationSyntax>();

            if (!methodDeclarations.Any())
            {
                return false;
            }

            for (int i = 0; i < methodDeclarations.Count(); i++)
            {
                var method = methodDeclarations.ElementAt(i);
                if (method.Modifiers.Any(SyntaxKind.PublicKeyword) && method.Modifiers.Any(SyntaxKind.StaticKeyword))
                {
                    var classDeclaration = _classTree.DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault();

                    if (!(method.ReturnType is IdentifierNameSyntax methodReturnType))
                    {
                        return false;
                    }

                    if (methodReturnType.Identifier.Value.Equals(classDeclaration.Identifier.Value))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool HasPrivateConstructor()
        {
            var constructorDeclarations = _classTree.DescendantNodes().OfType<ConstructorDeclarationSyntax>();

            if (!constructorDeclarations.Any())
            {
                return false;
            }

            for (int i = 0; i < constructorDeclarations.Count(); i++)
            {
                var constructor = constructorDeclarations.ElementAt(i);
                if (constructor.Modifiers.Any(SyntaxKind.PrivateKeyword))
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasPrivateStaticSelfField()
        {
            var fieldDeclarations = _classTree.DescendantNodes().OfType<FieldDeclarationSyntax>();

            if (!fieldDeclarations.Any())
            {
                return false;
            }

            var classDeclaration = _classTree.DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault();

            for (int i = 0; i < fieldDeclarations.Count(); i++)
            {
                var field = fieldDeclarations.ElementAt(i);

                if (!(field.Declaration.Type is IdentifierNameSyntax type))
                {
                    return false;
                }

                if (field.Modifiers.Any(SyntaxKind.PrivateKeyword) && field.Modifiers.Any(SyntaxKind.StaticKeyword) && type.Identifier.Value.Equals(classDeclaration.Identifier.Value))
                {
                    return true;
                }
            }

            return false;
        }

        public Location GetLocation()
        {
            return location;
        }
    }
}
