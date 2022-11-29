using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Simplification;
using InspectorPatterns.Analyzers;
using InspectorPatterns;

namespace InspectorPatterns
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AnalyzerCodeFixProvider)), Shared]
    public class AnalyzerCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(MethodAnalyzer.DiagnosticId, SingletonAnalyzer.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            // TODO: Replace the following code with your own analysis, generating a CodeAction for each fix to suggest
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            if (diagnostic.Id == MethodAnalyzer.DiagnosticId)
            {
                // Find the type declaration identified by the diagnostic.
                var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<CompilationUnitSyntax>().First();

                // Register a code action that will invoke the fix.
                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: CodeFixResources.MethodFixTitle,
                        createChangedDocument: c => MakePublicAsync(context.Document, declaration, c),
                        equivalenceKey: nameof(CodeFixResources.MethodFixTitle)),
                    diagnostic);
            }
        }

        private static async Task<Document> MakePublicAsync(Document document, CompilationUnitSyntax classTree, CancellationToken cancellationToken)
        {
            //// Remove the leading trivia from the local declaration.
            //MethodDeclarationSyntax methodDeclaration = classTree.DescendantNodes().OfType<MethodDeclarationSyntax>().FirstOrDefault();

            //SyntaxTriviaList leadingTrivia = firstToken.LeadingTrivia;
            //LocalDeclarationStatementSyntax trimmedLocal = methodDeclaration.Identifier();

            //// Create a const token with the leading trivia.
            //SyntaxToken constToken = SyntaxFactory.Token(leadingTrivia, SyntaxKind.ConstKeyword, SyntaxFactory.TriviaList(SyntaxFactory.ElasticMarker));

            //// Insert the const token into the modifiers list, creating a new modifiers list.
            //SyntaxTokenList newModifiers = trimmedLocal.Modifiers.Insert(0, constToken);

            //// Add an annotation to format the new local declaration.
            //LocalDeclarationStatementSyntax formattedLocal = newLocal.WithAdditionalAnnotations(Formatter.Annotation);



            // Replace the old local declaration with the new local declaration.
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            //SyntaxNode newRoot = oldRoot.ReplaceNode(classTree, formattedLocal);

            // Return document with transformed tree.
            return document.WithSyntaxRoot(oldRoot);
        }
    }
}
