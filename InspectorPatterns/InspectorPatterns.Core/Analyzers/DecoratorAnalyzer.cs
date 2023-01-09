using InspectorPatterns.Core.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Linq;


namespace InspectorPatterns.Core.Analyzers
{
    /**
     * Heeft de class een abstract class.
     * Is de constructor public en komt die overeen met de abstract class.
     * Is er een public propperty met de zelfde abstract class
     * heeft een constructor de zelfde abstract class als argument 
     */

    public class DecoratorAnalyzer
    {
        private static string _identifierValue;

        public class HasAbstractClass : IAnalyzer
        {
            private readonly SyntaxNodeAnalysisContext _context;


            public HasAbstractClass(SyntaxNodeAnalysisContext context)
            {
                _context = context;
            }

            public bool Analyze()
            {

                ClassDeclarationSyntax classSyntax = (ClassDeclarationSyntax)_context.Node;
                if (null == classSyntax.BaseList)
                {
                    return false;
                }
                // lists interfaces that current class have implemented
                var implementedInterfaces = classSyntax.BaseList.Types;
                foreach (var implementedInterface in implementedInterfaces)
                {
                    // gets all classes in a project
                    var allClasses = _context.Compilation.SyntaxTrees;
                    foreach (var klass in allClasses)
                    {
                        // gets declared classes
                        var classDeclarations = klass.GetRoot().DescendantNodesAndSelf(n => n is CompilationUnitSyntax || n is MemberDeclarationSyntax).OfType<ClassDeclarationSyntax>();

                        try
                        {
                            var classDeclarationBaseList = classDeclarations.First().BaseList;
                            if (null != classDeclarationBaseList)
                            {
                                var classListDeclarationTypes = classDeclarationBaseList.Types;
                                foreach (var classDeclarationType in classListDeclarationTypes)
                                {
                                    try
                                    {
                                        var type = (IdentifierNameSyntax)classDeclarationType.Type;
                                        if ((IdentifierNameSyntax)implementedInterface.Type != type)
                                        {
                                            continue;
                                        }
                                        else
                                        {
                                            DecoratorAnalyzer._identifierValue = type.ToString();
                                            return true;
                                        }
                                    }
                                    catch {
                                        continue;
                                    }


                                }
                            }
                        } catch(System.InvalidOperationException) { }
                    }
                }

                return false;
            }

            public string GetInterfaceName()
            {
                return DecoratorAnalyzer._identifierValue;
            }
        }

        public class HasPrivateStaticField : IAnalyzer
        {
            private readonly SyntaxNode _classTree;

            public HasPrivateStaticField(SyntaxNode context)
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

                    if (field.Modifiers.Any(SyntaxKind.PrivateKeyword) && type.Identifier.Value.Equals(DecoratorAnalyzer._identifierValue))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public class HasConstructor : IAnalyzer
        {
            private readonly SyntaxNode _classTree;

            public HasConstructor(SyntaxNode context)
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
                    if (constructor.ParameterList.Parameters.Count == 0)
                    {
                        continue;
                    }

                    var value = constructor.ParameterList.Parameters.First();

                    if (value == null)
                    {
                        continue;
                    }

                    if (constructor.Modifiers.Any(SyntaxKind.PublicKeyword) && value.Type.ToString() == DecoratorAnalyzer._identifierValue)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public class AnalyzeStatement : IAnalyzer
        {
            private readonly SyntaxNodeAnalysisContext _context;

            public AnalyzeStatement(SyntaxNodeAnalysisContext context)
            {
                _context = context;
            }

            public bool Analyze()
            {
                var objectCreation = _context.Node.DescendantNodes().OfType<ObjectCreationExpressionSyntax>();

                if (objectCreation.Count() == 0)
                {
                    return false;
                }

                if (objectCreation.First().ArgumentList.Arguments.Count() == 0)
                {
                    return false;
                }


                if (objectCreation.First().ArgumentList.Arguments.First().Expression is ObjectCreationExpressionSyntax)
                {
                    return true;
                }

                return false;
            }
        }
    }
}
