using InspectorPatterns.Core.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace InspectorPatterns.Core.Analyzers
{
    public class FlyweightAnalyzer
    {
        public static List<INamedTypeSymbol> collections = new List<INamedTypeSymbol>();

        /// <summary>
        /// Class used to find collections used for Flyweight
        /// </summary>
        public class HasFlyweightCollection : IAnalyzer
        {
            private readonly SyntaxNodeAnalysisContext _context;

            public HasFlyweightCollection(SyntaxNodeAnalysisContext context)
            {
                _context = context;
            }

            public bool Analyze()
            {
                // Checks if there are any field declerations which use interface ICollection (List, Dictionary, Enumerable, Array etc..).
                if (CheckFlyweightCollections() && collections.Count > 0)
                {
                    return true;
                }

                // If no Flyweight collections have been found, return false.
                return false;
            }

            /// <summary>
            /// Store all collections with containing type Flyweight class and return true if any have been found.
            /// </summary>
            /// <returns><see langword="true" /> if any collections with containing type Flyweight class have been found.<br/>
            /// <see langword="false" /> if none are found.</returns>
            public bool CheckFlyweightCollections()
            {
                List<INamedTypeSymbol> collectionList = new List<INamedTypeSymbol>();
                var fieldDeclerations = _context.Node.SyntaxTree.GetRoot().DescendantNodes().OfType<FieldDeclarationSyntax>();

                // Iterate through all field declerations.
                foreach (var item in fieldDeclerations)
                {
                    // Find all declerations of types that interface ICollection (List, Dictionary, Enumerable, Array etc..).
                    List<INamedTypeSymbol> collectionInterfaces = new List<INamedTypeSymbol>();

                    try
                    {
                         collectionInterfaces = _context.SemanticModel.GetTypeInfo(item.Declaration.Type).ConvertedType.Interfaces.Where(x => x.MetadataName == "ICollection").ToList();
                    } catch (Exception ex)
                    {
                        return false;
                    }

                    // Check if any have been found.
                    if (collectionInterfaces.Count == 0)
                    {
                        return false;
                    }

                    // Add found collections to a temporary list (This prevents O(N^2)).
                    collectionList.AddRange(collectionInterfaces);
                }

                // Iterate through all found collections in temporary list.
                foreach (INamedTypeSymbol collection in collectionList)
                {
                    // Check if collection has storable type of Flyweight class
                    if (!IsCollectionFlyweight(collection))
                    {
                        continue;
                    }

                    // Finally add found collection of which we know is used for Flyweight pattern to a global List.
                    collections.Add(collection);
                }

                // If any collection has been found that is used for Flyweight return true.
                if (collections.Count > 0)
                {
                    return true;
                }

                // If no collections have been found, return false.
                return false;
            }

            /// <summary>
            /// Check if collection contains objects of type Flyweight class. <br />
            /// TODO: This method has not been finished due to limitations in our knowledge of the Roslyn API. <br/>
            /// We have not been able to find a workaround and in agreement with the Stakeholders of InspectorPatterns we have decided to research this further in future.
            /// </summary>
            /// <param name="collection"></param>
            /// <returns><see langword="true" /> if collection if of type Flyweight class.<br/>
            /// <see langword="false" /> if collection is not of type Flyweight class.</returns>
            public bool IsCollectionFlyweight(INamedTypeSymbol collection)
            {
                // TODO: Check if collection can store type of Flyweight class.
                // TODO: Check if collection is distinct.
                return true;
            }
        }

        /// <summary>
        /// Class used to find methods used for Flyweight
        /// </summary>
        public class HasGetFlyweightMethod : IAnalyzer
        {
            private readonly MethodDeclarationSyntax _methodDeclaration;

            public HasGetFlyweightMethod(SyntaxNodeAnalysisContext context)
            {
                _methodDeclaration = (MethodDeclarationSyntax)context.Node;
            }

            public HasGetFlyweightMethod(MethodDeclarationSyntax methodDeclaration)
            {
                _methodDeclaration = methodDeclaration;
            }

            public bool Analyze()
            {
                // Check if method is public, not empty and return type is not null.
                if (!_methodDeclaration.Modifiers.Any(SyntaxKind.PublicKeyword) || _methodDeclaration.ReturnType == null || _methodDeclaration.Body == null)
                {
                    return false;
                }

                // Als wij een if statement hebben waarin een object wordt gemaakt van de class welke return type is van de method
                // Flyweight

                List<IfStatementSyntax> expressions = _methodDeclaration.Body.DescendantNodes().OfType<IfStatementSyntax>().ToList();
                List<ObjectCreationExpressionSyntax> objectCreations = new List<ObjectCreationExpressionSyntax>();
                
                foreach (IfStatementSyntax expression in expressions)
                {
                    objectCreations = expression.DescendantNodes().OfType<ObjectCreationExpressionSyntax>().ToList();
                }

                if (objectCreations.Count == 0)
                {
                    return false;
                }

                foreach (ObjectCreationExpressionSyntax objectCreation in objectCreations)
                {
                    // If method return type is equal to object creation type
                    // Unit test context can differ from actual environment.
                    var returnTypeINS = _methodDeclaration.ReturnType as IdentifierNameSyntax;
                    var returnTypePTS = _methodDeclaration.ReturnType as PredefinedTypeSyntax;
                    string returnType;

                    var creationTypeINS = objectCreation.Type as IdentifierNameSyntax;
                    var creationTypePTS = objectCreation.Type as PredefinedTypeSyntax;
                    string creationType;

                    // Check if any returnType is valid.
                    if (returnTypeINS == null)
                    {
                        if (returnTypePTS == null)
                        {
                            continue;
                        }
                        else
                        {
                            returnType = returnTypePTS.Keyword.ToString();
                        }
                    }
                    else
                    {
                        returnType = returnTypeINS.Identifier.ValueText;
                    }

                    // Check if any creationType is valid.
                    if (creationTypeINS == null)
                    {
                        if (creationTypePTS == null)
                        {
                            continue;
                        }
                        else
                        {
                            creationType = creationTypePTS.Keyword.ToString();
                        }
                    }
                    else
                    {
                        creationType = creationTypeINS.Identifier.ValueText;
                    }

                    // Check if either values are null, if they are, it is not Flyweight.
                    if (returnType == null || creationType == null)
                    {
                        continue;
                    }

                    // If Method ReturnType and CreationType are equal this might be Flyweight.
                    if (returnType == creationType)
                    {
                        return true;
                    }

                    // No Flyweight found.
                    return false;
                }

                // No Flyweight found.
                return false;
            }
        }
    }
}
