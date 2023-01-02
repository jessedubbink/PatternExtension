using InspectorPatterns.Core.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Linq;

namespace InspectorPatterns.Core.Analyzers
{
    public class DecoratorAnalyzer
    {

        public class HasSomeInterface : IAnalyzer
        {
            private readonly SyntaxNode _classTree;
            private readonly SyntaxNodeAnalysisContext _context;
            private string _identifierValue;


            public HasSomeInterface(SyntaxNodeAnalysisContext context)
            {
                _context = context;
            }

            public bool Analyze()
            {
                /**
                 * Heeft de class een interface.
                 * Is de constructor public en komt die overeen met de interface.
                 * Is er een public propperty met de zelfde interface
                 * heeft een constructor de zelfde interface als argument 
                 */
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
                                    var type = (IdentifierNameSyntax)classDeclarationType.Type;
                                    // checks if current interface is same as implemented interface of declared class in project
                                    if ((IdentifierNameSyntax)implementedInterface.Type != type)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        _identifierValue = type.ToString();
                                    }


                                }
                            }
                        } catch(System.InvalidOperationException) { }
                    }
                }

                return true;
            }

            public string GetInterfaceName()
            {
                return _identifierValue;
            }
        }
    }
}
