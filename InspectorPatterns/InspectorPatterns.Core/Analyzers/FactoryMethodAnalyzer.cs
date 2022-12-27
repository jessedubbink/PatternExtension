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
    public class FactoryMethodAnalyzer : IAnalyzer
    {
        private readonly SyntaxNode _classTree;

        public FactoryMethodAnalyzer(SyntaxNode context)
        {
            _classTree = context;
        }

        public bool Analyze()
        {
            //var methodDeclarations = _classTree.DescendantNodes().OfType<MethodDeclarationSyntax>();
            //foreach (var methodDecleration in methodDeclarations)
            //{
            //    var isFactoryMethod = IsFactoryMethod(methodDecleration);
            //}
            

            return true;
        }

        // Returns true if the member is a factory method, false otherwise
        public class IsFactoryMethod : IAnalyzer
        {
            private readonly SyntaxNodeAnalysisContext _context;

            public IsFactoryMethod(SyntaxNodeAnalysisContext context)
            {
                _context = context;
            }

            public bool Analyze()
            {
                MethodDeclarationSyntax methodSyntax = (MethodDeclarationSyntax)_context.Node;
                // Check if method is abstract and return type is not void
                if (!methodSyntax.Modifiers.Any(SyntaxKind.AbstractKeyword) && !methodSyntax.Modifiers.Any(SyntaxKind.VoidKeyword))
                {
                    return false;
                }

                // Check if the method creates objects and delegates object creation to subclasses
                // Check if the method is abstract and has a return type that is not void.
                if (
                    !((methodSyntax.ReturnType.GetType() != typeof(PredefinedTypeSyntax))
                    && methodSyntax.Modifiers.Any(SyntaxKind.AbstractKeyword)
                    && !methodSyntax.Modifiers.Any(SyntaxKind.VoidKeyword))
                    )
                {
                    return false;

                    // Alternatively, you could also check for use of reflection to create instances of classes
                }

                // Check if the method does not has a body, otherwise check if body is empty
                if (methodSyntax.Body == null || !methodSyntax.Body.DescendantNodes().Any())
                {
                    // This method may be a abstract factory method
                    return true;
                }


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

                return false;
            }
        }

        // Returns true if the node uses the factory method to create objects, false otherwise
        public class UsesFactoryMethod : IAnalyzer
        {
            private readonly SyntaxNodeAnalysisContext _context;

            public UsesFactoryMethod(SyntaxNodeAnalysisContext context)
            {
                _context = context;
            }

            public bool Analyze()
            {
                MethodDeclarationSyntax methodSyntax = (MethodDeclarationSyntax)_context.Node;

                // Check if the method overrides an abstract method and has a return type that is not void.
                if (
                    !(methodSyntax.ReturnType.GetType() != typeof(PredefinedTypeSyntax)
                    && methodSyntax.Modifiers.Any(SyntaxKind.OverrideKeyword)
                    && !methodSyntax.Modifiers.Any(SyntaxKind.VoidKeyword))
                    )
                {
                    return false;
                    // Alternatively, you could also check for use of reflection to create instances of classes
                }

                // Check if the method body contains a "new" expression
                if (!methodSyntax.Body.DescendantNodes().OfType<ObjectCreationExpressionSyntax>().Any())
                {
                    // This method may belong to a subclass that implements a factory method
                    return false;
                }

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
