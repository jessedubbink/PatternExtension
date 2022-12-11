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
        public Location Location { get; set; }

        public int MyProperty { get; set; }

        public SingletonAnalyzer(SyntaxNodeAnalysisContext context)
        {
            _context = context;
        }

        public bool Analyze()
        {
            if (HasGetInstance() && HasPrivateConstructor() && HasPrivateStaticSelf())
            {
                var classTree = _context.Node.SyntaxTree.GetRoot();
                Location = classTree.DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault().Identifier.GetLocation();

                return true;
            }

            return false;
        }

        public bool HasGetInstance()
        {
            var classTree = _context.Node.SyntaxTree.GetRoot();
            var methodDeclarations = classTree.DescendantNodes().OfType<MethodDeclarationSyntax>();

            if (!methodDeclarations.Any())
            {
                return false;
            }

            for (int i = 0; i < methodDeclarations.Count(); i++)
            {
                var method = methodDeclarations.ElementAt(i);
                if (method.Modifiers.Any(SyntaxKind.PublicKeyword) && method.Modifiers.Any(SyntaxKind.StaticKeyword))
                {
                    var methodReturnType = method.ReturnType as IdentifierNameSyntax;
                    var classDeclaration = classTree.DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault();

                    if (methodReturnType == null)
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
            var classTree = _context.Node.SyntaxTree.GetRoot();
            var constructorDeclarations = classTree.DescendantNodes().OfType<ConstructorDeclarationSyntax>();

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

        public bool HasPrivateStaticSelf()
        {
            var classTree = _context.Node.SyntaxTree.GetRoot();
            var fieldDeclarations = classTree.DescendantNodes().OfType<FieldDeclarationSyntax>();
            var classDeclaration = classTree.DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault();

            if (!fieldDeclarations.Any())
            {
                return false;
            }

            for (int i = 0; i < fieldDeclarations.Count(); i++)
            {
                var field = fieldDeclarations.ElementAt(i);

                var type = field.Declaration.Type as IdentifierNameSyntax;
                if (field.Modifiers.Any(SyntaxKind.PrivateKeyword) && field.Modifiers.Any(SyntaxKind.StaticKeyword) && type.Identifier.Value.Equals(classDeclaration.Identifier.Value))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
