using InspectorPatterns.Core.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace InspectorPatterns.Core.Analyzers
{
    public class SingletonAnalyzer
    {
        public class HasGetInstanceSelfMethod : IAnalyzer
        {
            private readonly SyntaxNode _classTree;

            public HasGetInstanceSelfMethod(SyntaxNode context)
            {
                _classTree = context;
            }

            public bool Analyze()
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
                            continue;
                        }

                        if (methodReturnType.Identifier.Value.Equals(classDeclaration.Identifier.Value))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        public class HasPrivateConstructor : IAnalyzer
        {
            private readonly SyntaxNode _classTree;

            public HasPrivateConstructor(SyntaxNode context)
            {
                _classTree = context;
            }

            public bool Analyze()
            {
                var constructorDeclarations = _classTree.DescendantNodes().OfType<ConstructorDeclarationSyntax>();

                if (!constructorDeclarations.Any())
                {
                    return false;
                }

                for (int i = 0; i < constructorDeclarations.Count(); i++)
                {
                    var constructor = constructorDeclarations.ElementAt(i);

                    if (!constructor.Modifiers.Any(SyntaxKind.PublicKeyword) && constructor.Modifiers.Any(SyntaxKind.PrivateKeyword))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public class HasPrivateStaticSelfField : IAnalyzer
        {
            private readonly SyntaxNode _classTree;

            public HasPrivateStaticSelfField(SyntaxNode context)
            {
                _classTree = context;
            }

            public bool Analyze()
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
                        continue;
                    }

                    if (field.Modifiers.Any(SyntaxKind.PrivateKeyword) && field.Modifiers.Any(SyntaxKind.StaticKeyword) && type.Identifier.Value.Equals(classDeclaration.Identifier.Value))
                    {
                        return true;
                    }
                }

                return false;
            }
        }
    }
}
