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
            private readonly SyntaxNodeAnalysisContext _context;

            public IsAbstractFactoryMethod(SyntaxNodeAnalysisContext context)
            {
                _context = context;
            }

            // Returns true if the member is an abstract factory method, false otherwise
            public bool Analyze()
            {
                MethodDeclarationSyntax methodDeclaration = (MethodDeclarationSyntax)_context.Node;
                // Returns false if method is not abstract
                if (!methodDeclaration.Modifiers.Any(SyntaxKind.AbstractKeyword))
                {
                    return false;
                }

                // Returns false if method creates predefined objects like: string, int, bool, void etc.
                if (methodDeclaration.ReturnType.GetType() == typeof(PredefinedTypeSyntax))
                {
                    return false;
                }

                // TODO:
                // Check if returntype is Product Interface

                //// Check if the method does has a body and if body is empty
                //if (methodDeclaration.Body != null && methodDeclaration.Body.DescendantNodes().Any())
                //{
                //    // This method may be a abstract factory method
                //    return false;
                //}


                //var methodDeclarations = _context.DescendantNodes().OfType<MethodDeclarationSyntax>();
                //foreach (var method in methodDeclarations)
                //{
                //    // Check if method is abstract and return type is not void
                //    if (!method.Modifiers.Any(SyntaxKind.AbstractKeyword) && !method.Modifiers.Any(SyntaxKind.VoidKeyword))
                //    {
                //        return false;
                //    }

                //    // Check if the method creates objects and delegates object creation to subclasses
                //    // Check if the method is abstract and has a return type that is not void.
                //    if (method.ReturnType.GetType() != typeof(PredefinedTypeSyntax) && method.Modifiers.Any(SyntaxKind.AbstractKeyword) && !method.Modifiers.Any(SyntaxKind.VoidKeyword))
                //    {
                //        // Check if the method does not has a body, otherwise check if body is empty
                //        if (method.Body == null || !method.Body.DescendantNodes().Any())
                //        {
                //            // This method may be a abstract factory method
                //            return true;
                //        }

                //        // Alternatively, you could also check for use of reflection to create instances of classes
                //    }

                //    return false;
                //}

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
                //InterfaceDeclarationSyntax interfaceSyntax = (InterfaceDeclarationSyntax)_context.Node;

                //var x = interfaceSyntax.SyntaxTree.GetCompilationUnitRoot();

                //var y = _context.Compilation.References;

                ////foreach (var syntaxTree in _context.Compilation.SyntaxTrees)
                ////{
                ////    SemanticModel model = _context.Compilation.GetSemanticModel(syntaxTree);
                ////    var symbol = _context.Compilation.GetSymbolsWithName(interfaceSyntax.Identifier.Text).First();
                ////    //var symbol = model.GetSymbolInfo(_context.Node).Symbol;
                ////    var t = symbol?.DeclaringSyntaxReferences;
                ////}
                //var semanticModel = _context.Compilation.GetSemanticModel(_context.Node.SyntaxTree);
                //var clssymbol = semanticModel.GetDeclaredSymbol(_context.Node);

                //var interfaces = clssymbol.Locations;

                //var symbol = _context.Compilation.GetSymbolsWithName(interfaceSyntax.Identifier.Text).First();
                ////var symbol = model.GetSymbolInfo(_context.Node).Symbol;
                //var t = symbol?.DeclaringSyntaxReferences;

                ////symbol

                ////if (x != null)
                ////{
                ////    return false;
                ////}


                //if (!ProductInterfaceUsedAsReturnTypeWithFactoryMethodDeclaration(interfaceSyntax.Identifier.Value))
                //{
                //    return false;
                //}

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

            //// Returns true if Product interface is used as a return type with Factory Method declaration, for example: IProduct FactoryMethod(). Otherwise false
            //public bool ProductInterfaceUsedAsReturnTypeWithFactoryMethodDeclaration(object value)
            //{
            //    foreach (var syntaxTree in _context.Compilation.SyntaxTrees)
            //    {
            //        var classDeclarations = syntaxTree.GetRoot().DescendantNodesAndSelf(n => n is CompilationUnitSyntax || n is MemberDeclarationSyntax).OfType<ClassDeclarationSyntax>();
                    
            //        if (!classDeclarations.Any())
            //        {
            //            continue;
            //        }

            //        var methodDeclarations = classDeclarations.First().DescendantNodes().OfType<MethodDeclarationSyntax>();

            //        foreach (var method in methodDeclarations)
            //        {
            //            if (!(method.ReturnType is IdentifierNameSyntax methodReturnType))
            //            {
            //                continue;
            //            }

            //            if (methodReturnType.Identifier.Value.Equals(value))
            //            {
            //                return true;
            //            }
            //        }                    
            //    }

            //    return false;
            //}
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
                var localDeclaration = (LocalDeclarationStatementSyntax)_context.Node;
                var localSementicModel = _context.Compilation.GetSemanticModel(_context.Node.SyntaxTree);
                //var x = localSementicModel.GetDeclaredSymbol(localDeclaration.Declaration);//.Variables.First();
                var localDeclaredSymbol = (ILocalSymbol)localSementicModel.GetDeclaredSymbol(localDeclaration.Declaration.Variables.First());
                //var localVariable = localDeclaredSymbol as ILocalSymbol;

                // Return false if the Type of local variable is a predefined object, like: string, int, bool, void etc.
                if (localDeclaredSymbol.Type.GetType() == typeof(PredefinedTypeSyntax))
                {
                    return false;
                }

                //var invocationExpressions = _context.Node.DescendantNodes().OfType<InvocationExpressionSyntax>();
                // The method that is used for declaring the variable.
                var invocationExpressions = _context.Node.DescendantNodes().OfType<InvocationExpressionSyntax>();
                if (!invocationExpressions.Any())
                {
                    return false;
                }
                // Name of the method.
                var identifierName = invocationExpressions.First().DescendantNodes().OfType<IdentifierNameSyntax>().First();
                // Filter the Syntax Tree where the abstract method originally is declared. 
                var syntaxTrees = _context.Compilation.SyntaxTrees
                    .Where(s => s.GetRoot()
                        .DescendantNodes()
                        .OfType<MethodDeclarationSyntax>()
                        .Any(m => m.Identifier.Value == identifierName.Identifier.Value && m.Modifiers.Any(SyntaxKind.AbstractKeyword)));
                //var h = syntaxTrees.Where(s => s.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>().Where(m => m.Identifier.Value == y.Identifier.Value) != null).Select(n => n.GetRoot());
                //var p = syntaxTrees.Select(s => s.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>()).Where(m => m.Any(n => n.Identifier.Value == y.Identifier.Value));
                //var p = syntaxTrees.Select(s => s.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>()).Where(m => m.Any(n => n.Identifier.Value == y.Identifier.Value && n.ReturnType.GetType() != typeof(PredefinedTypeSyntax) && n.Modifiers.Any(SyntaxKind.AbstractKeyword)));
                //var l = syntaxTrees.Where(s => s.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>().Any(m => m.Identifier.Value == y.Identifier.Value && m.Modifiers.Any(SyntaxKind.AbstractKeyword)));
                // .Where(n => n.Identifier.Value == y.Identifier.Value)
                //.Select(n => n.Identifier.Value == y.Identifier.Value)

                if (!syntaxTrees.Any()) return false;

                foreach (var syntaxTree in syntaxTrees)
                {
                    var methodDeclarations = syntaxTree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>();
                    var sModel = _context.Compilation.GetSemanticModel(syntaxTree);

                    if (!methodDeclarations.Any()) continue;

                    var methodDeclaration = methodDeclarations.Where(m => m.Identifier.Value == identifierName.Identifier.Value).First();
                    var declaredSymbol = sModel.GetDeclaredSymbol(methodDeclaration); ;// .GetSymbolInfo(_context.Node).Symbol;// .GetDeclaredSymbol(creator);

                    // Return false if the type of the variable is a predefined object, like: string, int, bool, void etc.
                    if (declaredSymbol.ReturnType.GetType() == typeof(PredefinedTypeSyntax))
                    {
                        return false;
                    }
                }

                // Object creation is achieved by using a Factory Method.
                return true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        // Parse the source code file
        //private static string sourceCode = File.ReadAllText("MyFile.cs");
        //private static SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);

        //// Get the root node of the AST
        //static SyntaxNode root = syntaxTree.GetRoot();

        //// Get a collection of all the descendant nodes of the root node
        //IEnumerable<SyntaxNode> descendantNodes = root.DescendantNodes();

        //private void Test()
        //{
        //    // Iterate through the descendant nodes
        //    foreach (SyntaxNode node in descendantNodes)
        //    {
        //        // Check if the node is a class declaration
        //        if (node is ClassDeclarationSyntax classDeclaration)
        //        {
        //            // Check if the class has a factory method
        //            if (classDeclaration.Members.Any(m => IsFactoryMethod(m)))
        //            {
        //                // Check if there are any subclasses that override the factory method
        //                if (classDeclaration.DescendantNodes().Any(n => IsSubclass(n)))
        //                {
        //                    // Check if the client code uses the factory method to create objects
        //                    if (root.DescendantNodes().Any(n => UsesFactoryMethod(n)))
        //                    {
        //                        // The source code uses the factory method design pattern
        //                        Console.WriteLine("Factory method design pattern detected!");
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}



        // Returns true if the member is a factory method, false otherwise
        //private bool IsFactoryMethod(MethodDeclarationSyntax method)
        //{
        //    // Check if method is abstract and return type is not void
        //    if (!method.Modifiers.Any(SyntaxKind.AbstractKeyword) && !method.Modifiers.Any(SyntaxKind.VoidKeyword))
        //    {
        //        return false;
        //    }

        //    // Check if the method creates objects and delegates object creation to subclasses
        //    // Check if the method is abstract and has a return type that is not void.
        //    if (method.ReturnType.GetType() != typeof(PredefinedTypeSyntax) && method.Modifiers.Any(SyntaxKind.AbstractKeyword) && !method.Modifiers.Any(SyntaxKind.VoidKeyword))
        //    {
        //        // Check if the method body is empty
        //        if (!method.Body.DescendantNodes().Any())
        //        {
        //            // This method may be a abstract factory method
        //            return true;
        //        }

        //        // Alternatively, you could also check for use of reflection to create instances of classes
        //    }

        //    return false;
        //}

        // Returns true if the node is a subclass, false otherwise
        private static bool IsSubclass(SyntaxNode node)
        {
            // Check if the node is a class declaration and it derives from the factory class
            // (implementation details depend on the specific pattern being used)
            return false;
        }

        // Return true if the node is a product, false otherwise
        private bool IsProduct()
        {
            return false;
        }

        //// Returns true if the node uses the factory method to create objects, false otherwise
        //private bool UsesFactoryMethod(MethodDeclarationSyntax method)
        //{
        //    // Check if the method overrides an abstract method and has a return type that is not void.
        //    if (method.ReturnType.GetType() != typeof(PredefinedTypeSyntax) && method.Modifiers.Any(SyntaxKind.OverrideKeyword) && !method.Modifiers.Any(SyntaxKind.VoidKeyword))
        //    {
        //        // Check if the method body contains a "new" expression
        //        if (method.Body.DescendantNodes().OfType<ObjectCreationExpressionSyntax>().Any())
        //        {
        //            // This method may belong to a subclass that implements a factory method
        //            return true;
        //        }

        //        // Alternatively, you could also check for use of reflection to create instances of classes
        //    }

        //    return false;
        //}

        //void L()
        //{
        //    foreach (MethodDeclarationSyntax method in root.DescendantNodesAndSelf().OfType<MethodDeclarationSyntax>())
        //    {
        //        // Check if the method has a return type that is not void
        //        if (method.ReturnType is not PredefinedTypeSyntax returnType && returnType.Keyword.Text != "void")
        //        {
        //            // Check if the method body contains a "new" expression
        //            if (method.Body.DescendantNodes().OfType<ObjectCreationExpressionSyntax>().Any())
        //            {
        //                // This method may be a factory method
        //            }

        //            // Alternatively, you could also check for use of reflection to create instances of classes
        //        }
        //    }
        //}
    }
}
