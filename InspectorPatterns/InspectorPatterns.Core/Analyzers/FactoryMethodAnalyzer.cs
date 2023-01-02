using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using InspectorPatterns.Core.Interfaces;
using Microsoft.CodeAnalysis.Diagnostics;

namespace InspectorPatterns.Core.Analyzers
{
    public class FactoryMethodAnalyzer
    {
        public class IsAbstractFactoryMethod : IAnalyzer
        {
            private readonly MethodDeclarationSyntax _methodDeclaration;

            public IsAbstractFactoryMethod(SyntaxNodeAnalysisContext context)
            {
                _methodDeclaration = (MethodDeclarationSyntax)context.Node;
            }

            public IsAbstractFactoryMethod(MethodDeclarationSyntax methodDeclaration)
            {
                this._methodDeclaration = methodDeclaration;
            }

            // Returns true if the member is an abstract factory method, false otherwise
            public bool Analyze()
            {
                // Returns false if method is not abstract
                if (!_methodDeclaration.Modifiers.Any(SyntaxKind.AbstractKeyword))
                {
                    return false;
                }

                // Returns false if method creates predefined objects like: string, int, bool, void etc.
                if (_methodDeclaration.ReturnType.GetType() == typeof(PredefinedTypeSyntax))
                {
                    return false;
                }

                // TODO:
                // Check if returntype is Product Interface

                return true;
            }
        }

        public class OverridesAbstractFactoryMethod : IAnalyzer
        {
            private readonly SyntaxNodeAnalysisContext _context;

            public OverridesAbstractFactoryMethod(SyntaxNodeAnalysisContext context)
            {
                _context = context;
            }

            // Returns true if the node uses the factory method to create objects, false otherwise
            public bool Analyze()
            {
                MethodDeclarationSyntax methodDeclaration = (MethodDeclarationSyntax)_context.Node;

                // Returns false if the method returns a predefined returntype, like: string, int, bool, void etc. and/or does not have an override modifier.
                if (methodDeclaration.ReturnType.GetType() == typeof(PredefinedTypeSyntax) || !methodDeclaration.Modifiers.Any(SyntaxKind.OverrideKeyword))
                {
                    return false;
                }

                if (methodDeclaration.Body == null) return false;

                // Return false if the method body does not contains a "new" expression.
                if (!methodDeclaration.Body.DescendantNodes().OfType<ObjectCreationExpressionSyntax>().Any())
                {
                    return false;
                }

                // Node may belong to a subclass that implements a factory method
                return true;
            }
        }

        public class IsProductInterface : IAnalyzer
        {
            private readonly SyntaxNodeAnalysisContext _context;
            private readonly string _identifierValue;

            public IsProductInterface(SyntaxNodeAnalysisContext context)
            {
                _context = context;

                InterfaceDeclarationSyntax interfaceSyntax = (InterfaceDeclarationSyntax)_context.Node;
                _identifierValue = (string)interfaceSyntax.Identifier.Value;
            }

            public IsProductInterface(SyntaxNodeAnalysisContext context, string identifierValue)
            {
                _context = context;
                _identifierValue = identifierValue;
            }


            // Returns true if interface is used as a return type with Factory Method declaration, for example: IProduct FactoryMethod(). Otherwise false
            public bool Analyze()
            {
                foreach (var syntaxTree in _context.Compilation.SyntaxTrees)
                {
                    var classDeclarations = syntaxTree.GetRoot().DescendantNodesAndSelf(n => n is CompilationUnitSyntax || n is MemberDeclarationSyntax).OfType<ClassDeclarationSyntax>();

                    if (!classDeclarations.Any())
                    {
                        continue;
                    }

                    var methodDeclarations = classDeclarations.First().DescendantNodes().OfType<MethodDeclarationSyntax>();

                    foreach (var method in methodDeclarations)
                    {
                        if (!(method.ReturnType is IdentifierNameSyntax methodReturnType))
                        {
                            continue;
                        }

                        if (methodReturnType.Identifier.Value.Equals(_identifierValue))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        public class IsConcreteProduct : IAnalyzer
        {
            private readonly SyntaxNodeAnalysisContext _context;

            public IsConcreteProduct(SyntaxNodeAnalysisContext context)
            {
                _context = context;
            }

            // Returns true if class implements a "Product" interface.
            public bool Analyze()
            {
                ClassDeclarationSyntax classDeclaration = (ClassDeclarationSyntax)_context.Node;

                if (classDeclaration.BaseList == null)
                {
                    return false;
                }

                foreach (var item in classDeclaration.BaseList.Types)
                {
                    var type = (IdentifierNameSyntax)item.Type;
                    var factoryMethodAnalyzer = new FactoryMethodAnalyzer.IsProductInterface(_context, type.Identifier.Value.ToString());

                    if (factoryMethodAnalyzer.Analyze())
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public class DeclarationWithFactoryMethod : IAnalyzer
        {
            private readonly SyntaxNodeAnalysisContext _context;

            public DeclarationWithFactoryMethod(SyntaxNodeAnalysisContext context)
            {
                _context = context;
            }

            // Returns true if object creation is achieved by using a Factory Method, otherwise false.
            public bool Analyze()
            {
                //var localDeclaration = (LocalDeclarationStatementSyntax)_context.Node;
                //TypeSyntax variableTypeName = localDeclaration.Declaration.Type;
                //ITypeSymbol variableType = _context.SemanticModel.GetTypeInfo(variableTypeName, _context.CancellationToken).ConvertedType;
                //TypeSyntax typeName = SyntaxFactory.ParseTypeName(variableType.Name);
                ////var symbolInfo = _context.SemanticModel.GetSymbolInfo(variableTypeName, _context.CancellationToken);
                ////var typeSymbol = symbolInfo.Symbol;

                ////var localSementicModel = _context.Compilation.GetSemanticModel(_context.Node.SyntaxTree);
                ////var localDeclaredSymbol = (ILocalSymbol)localSementicModel.GetDeclaredSymbol(localDeclaration.Declaration.Variables.First());

                //// Return false if the Type of local variable is a predefined object, like: string, int, bool, void etc.
                //if (typeName.GetType() == typeof(PredefinedTypeSyntax))
                //{
                //    return false;
                //}

                // The method that is used for declaring the variable.
                var invocationExpressions = _context.Node.DescendantNodes().OfType<InvocationExpressionSyntax>();
                if (!invocationExpressions.Any())
                {
                    return false;
                }
                // Name of the method.
                var identifierName = invocationExpressions.First().DescendantNodes().OfType<IdentifierNameSyntax>().First();
                // Filter the Syntax Tree where the abstract method originally is declared. 
                var syntaxTree = _context.Compilation.SyntaxTrees.FirstOrDefault(s => s
                    .GetRoot()
                    .DescendantNodes()
                    .OfType<MethodDeclarationSyntax>()
                    .Any(m => m.Identifier.Value == identifierName.Identifier.Value && m.Modifiers.Any(SyntaxKind.AbstractKeyword)));
                //.Where(s => s.GetRoot()
                //    .DescendantNodes()
                //    .OfType<MethodDeclarationSyntax>()
                //    .Any(m => m.Identifier.Value == identifierName.Identifier.Value && m.Modifiers.Any(SyntaxKind.AbstractKeyword)));

                //if (!syntaxTrees.Any()) return false;
                if (syntaxTree == null) return false;

                //foreach (var syntaxTree in syntaxTrees)
                //{
                var methodDeclaration = syntaxTree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>().FirstOrDefault(m => m.Identifier.Value == identifierName.Identifier.Value);

                //if (!methodDeclarations.Any()) return false;

                //var methodDeclaration = methodDeclarations.Where(m => m.Identifier.Value == identifierName.Identifier.Value).First();

                if (methodDeclaration == null) return false;

                var analyzer = new FactoryMethodAnalyzer.IsAbstractFactoryMethod(methodDeclaration);

                // Returns false if the method is not an abstract Factory Method.
                if (!analyzer.Analyze())
                {
                    return false;
                }
                //}

                // Object creation is achieved by using a Factory Method.
                return true;
            }
        }

        // TODO:
        // Returns true if the node is a subclass, false otherwise
    }
}
