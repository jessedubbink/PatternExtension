using InspectorPatterns.Core;
using InspectorPatterns.Core.Analyzers;
using InspectorPatterns.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace InspectorPatterns
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class FactoryMethodPatternAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "FactoryMethodPatternAnalyzer";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.FactoryMethodTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.FactoryMethodMessage), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.FactoryMethodDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Creational design pattern";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Info, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        private DesignPatternAnalyzer analyzer;

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            if (analyzer == null)
            {
                analyzer = new DesignPatternAnalyzer();
            }

            // TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
            // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
            //context.RegisterSyntaxNodeAction(AnalyzeFieldNode, SyntaxKind.FieldDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeNode_For_IsProductInterface, SyntaxKind.InterfaceDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeNode_For_IsAbstractFactoryMethod, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeNode_For_OverridesAbstractFactoryMethod, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeNode_For_IsConcreteProduct, SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeNode_For_Test, SyntaxKind.LocalDeclarationStatement);
            context.RegisterSyntaxNodeAction(AnalyzeNode_For_Property, SyntaxKind.PropertyDeclaration);
            //context.RegisterSyntaxNodeAction(Test, SyntaxKind.ClassDeclaration);
        }

        private void AnalyzeNode_For_Property(SyntaxNodeAnalysisContext obj)
        {
            throw new NotImplementedException();
        }

        private void AnalyzeNode_For_Test(SyntaxNodeAnalysisContext obj)
        {
            var y = obj.Compilation.GetSemanticModel(obj.Node.SyntaxTree);
            var localDeclaration = (LocalDeclarationStatementSyntax)obj.Node;
            var s = obj.SemanticModel.SyntaxTree.GetRoot();
            var p = localDeclaration.Declaration.Variables.First().Initializer.Value;
            var t = obj.Node.DescendantNodes().OfType<InvocationExpressionSyntax>();

            TypeSyntax variableTypeName = localDeclaration.Declaration.Type;
            ITypeSymbol variableType = obj.SemanticModel.GetTypeInfo(variableTypeName, obj.CancellationToken).ConvertedType;

            LocalDeclarationStatementSyntax interfaceSyntax = (LocalDeclarationStatementSyntax)obj.Node;
            //var classDeclarations = syntaxTree.GetRoot().DescendantNodesAndSelf(n => n is CompilationUnitSyntax || n is MemberDeclarationSyntax).OfType<ClassDeclarationSyntax>();
            var x = interfaceSyntax.DescendantNodes().OfType<InvocationExpressionSyntax>();//(n => n is MemberAccessExpressionSyntax);//.OfType<SyntaxToken>();//.DescendantNodes(n => n is InvocationExpressionSyntax).OfType<InvocationExpression>();
            var node = obj.Node;
            var syntaxTree = node.SyntaxTree;
            var root = node.SyntaxTree.GetRoot();
        }

        private void AnalyzeNode_For_IsConcreteProduct(SyntaxNodeAnalysisContext context)
        {
            analyzer.SetAnalyzerStrategy(new FactoryMethodAnalyzer.IsConcreteProduct(context));

            if (!analyzer.Analyze())
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Rule, context.Node.GetLocation()));
        }

        private void AnalyzeNode_For_IsProductInterface(SyntaxNodeAnalysisContext context)
        {
            analyzer.SetAnalyzerStrategy(new FactoryMethodAnalyzer.IsProductInterface(context));

            if (!analyzer.Analyze())
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Rule, context.Node.GetLocation()));
        }
        
        private void AnalyzeNode_For_OverridesAbstractFactoryMethod(SyntaxNodeAnalysisContext context)
        {
            analyzer.SetAnalyzerStrategy(new FactoryMethodAnalyzer.OverridesAbstractFactoryMethod(context));

            if (!analyzer.Analyze())
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Rule, context.Node.GetLocation()));
        }

        private void AnalyzeNode_For_IsAbstractFactoryMethod(SyntaxNodeAnalysisContext context)
        {
            analyzer.SetAnalyzerStrategy(new FactoryMethodAnalyzer.IsAbstractFactoryMethod(context));

            if (!analyzer.Analyze())
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Rule, context.Node.GetLocation()));
        }

        private void Test(SyntaxNodeAnalysisContext obj)
        {
            //// https://stackoverflow.com/questions/19001423/getting-path-to-the-parent-folder-of-the-solution-file-using-c-sharp
            //var path = Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).FullName);
            //var solutionName = Path.GetFileName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            //var x = System.Diagnostics.Process.GetProcesses();// .GetCurrentProcess().MainModule.FileName;

            //var solutionName2 = Path.GetFileName("InspectorPatterns.sln");
            //solutionName = solutionName.Replace("exe", "sln");
            //var targetPath = Path.Combine(path, solutionName);

            //MSBuildLocator.RegisterDefaults();
            //var workspace = MSBuildWorkspace.Create();
            //var sln = await workspace.OpenSolutionAsync(targetPath);

            //// https://stackoverflow.com/questions/18316683/how-to-get-the-current-project-name-in-c-sharp-code
            //string projectName = Assembly.GetExecutingAssembly().FullName.Split(',')[0];
            //Project project = sln.Projects.First(x => x.Name == projectName);
            //var compilation = await project.GetCompilationAsync();



            //var creator = obj.Node as ClassDeclarationSyntax;
            //var x = creator.BaseList.Types.First();//.Type.SyntaxTree.GetCompilationUnitRoot();
            ////throw new NotImplementedException();
            //var root = obj.Node.SyntaxTree.GetCompilationUnitRoot();

            //var compilation = CSharpCompilation.Create("HelloWorld")
            //    .AddReferences(MetadataReference.CreateFromFile(
            //        typeof(string).Assembly.Location))
            //    .AddSyntaxTrees(obj.Node.SyntaxTree);

            //SemanticModel model = compilation.GetSemanticModel(obj.Node.SyntaxTree);

            //var symbol = model.GetSymbolInfo(obj.Node).Symbol;// .GetDeclaredSymbol(creator);
            ////var c = p.AllInterfaces.First();
            //var y = compilation.GetTypeByMetadataName("EmulatorProject.DesignPatterns.FactoryMethod.Creator");
            //var symbolVisitor = new NamedTypeVisitor();
            //var walker = new CustomWalker();
            ////walker.Visit(c);
            ////symbolVisitor.VisitNamedType(c);
        }
    }
}
