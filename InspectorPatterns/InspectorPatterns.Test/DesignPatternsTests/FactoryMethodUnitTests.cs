using InspectorPatterns.Core.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Linq;
using System.Threading;
using Xunit;

namespace InspectorPatterns.Test.DesignPatternsTests
{
    public class FactoryMethodUnitTests
    {
        [Fact]
        public void Test_IsAbstractFactoryMethod_NoAbstractMethods_ShouldReturnFalse()
        {
            //Arrange
            var wrongCode = @"
                public abstract class Creator
                {
                    private string NoAbstractModifier()
                    {
                        throw new NotImplementedException();
                    }
                }";
            var contextNode = SyntaxFactory.ParseSyntaxTree(wrongCode).GetRoot();
            var methodDeclaration = contextNode.DescendantNodes().OfType<MethodDeclarationSyntax>().FirstOrDefault();
            var factoryMethodAnalyzer = new FactoryMethodAnalyzer.IsAbstractFactoryMethod(methodDeclaration);

            //Act
            var result = factoryMethodAnalyzer.Analyze();

            //Assert
            Assert.False(result);
        }

        [Fact]
        public void Test_IsAbstractFactoryMethod_NoMethodsWithNonePredefinedReturnType_ShouldReturnFalse()
        {
            //Arrange
            var wrongCode = @"
                public abstract class Creator
                {
                    private abstract string PredefinedReturnType();
                }";
            var contextNode = SyntaxFactory.ParseSyntaxTree(wrongCode).GetRoot();
            var methodDeclaration = contextNode.DescendantNodes().OfType<MethodDeclarationSyntax>().FirstOrDefault();
            var factoryMethodAnalyzer = new FactoryMethodAnalyzer.IsAbstractFactoryMethod(methodDeclaration);

            //Act
            var result = factoryMethodAnalyzer.Analyze();

            //Assert
            Assert.False(result);
        }

        [Fact]
        public void Test_IsAbstractFactoryMethod_ShouldSucceed()
        {
            //Arrange
            var correctCode = @"
                public abstract class Creator
                {
                    private abstract IProduct FactoryMethod();
                }";
            var contextNode = SyntaxFactory.ParseSyntaxTree(correctCode).GetRoot();
            var methodDeclaration = contextNode.DescendantNodes().OfType<MethodDeclarationSyntax>().FirstOrDefault();
            var factoryMethodAnalyzer = new FactoryMethodAnalyzer.IsAbstractFactoryMethod(methodDeclaration);

            //Act
            var result = factoryMethodAnalyzer.Analyze();

            //Assert
            Assert.True(result);
        }

        [Fact]
        public void Test_OverridesAbstractFactoryMethod_NoMethodsWithNonePredefinedReturnType_ShouldReturnFalse()
        {
            //Arrange
            var wrongCode = @"
                public class ConcreteCreator1 : Creator
                {
                    public override string PredefinedReturnType()
                    {
                        throw new NotImplementedException();
                    }
                }";
            var syntaxTree = SyntaxFactory.ParseSyntaxTree(wrongCode).GetRoot();
            var methodDeclaration = syntaxTree.DescendantNodes().OfType<MethodDeclarationSyntax>().FirstOrDefault();
            var cToken = new CancellationToken();
            var context = new SyntaxNodeAnalysisContext(methodDeclaration, null, null, null, null, cToken);
            var factoryMethodAnalyzer = new FactoryMethodAnalyzer.OverridesAbstractFactoryMethod(context);

            //Act
            var result = factoryMethodAnalyzer.Analyze();

            //Assert
            Assert.False(result);
        }

        [Fact]
        public void Test_OverridesAbstractFactoryMethod_NoMethodsWithOverrideModifier_ShouldReturnFalse()
        {
            //Arrange
            var wrongCode = @"
                public class ConcreteCreator1 : Creator
                {
                    public IProduct NoOverrideModifier()
                    {
                        throw new NotImplementedException();
                    }
                }";
            var syntaxTree = SyntaxFactory.ParseSyntaxTree(wrongCode).GetRoot();
            var methodDeclaration = syntaxTree.DescendantNodes().OfType<MethodDeclarationSyntax>().FirstOrDefault();
            var cToken = new CancellationToken();
            var context = new SyntaxNodeAnalysisContext(methodDeclaration, null, null, null, null, cToken);
            var factoryMethodAnalyzer = new FactoryMethodAnalyzer.OverridesAbstractFactoryMethod(context);

            //Act
            var result = factoryMethodAnalyzer.Analyze();

            //Assert
            Assert.False(result);
        }

        [Fact]
        public void Test_OverridesAbstractFactoryMethod_MethodBodyDoesNotContainNewExpression_ShouldReturnFalse()
        {
            //Arrange
            var wrongCode = @"
                public class ConcreteCreator1 : Creator
                {
                    public override ConcreteCreator1 DoesNotContainNewExpression()
                    {
                        
                    }
                }";
            var syntaxTree = SyntaxFactory.ParseSyntaxTree(wrongCode).GetRoot();
            var methodDeclaration = syntaxTree.DescendantNodes().OfType<MethodDeclarationSyntax>().FirstOrDefault();
            var cToken = new CancellationToken();
            var context = new SyntaxNodeAnalysisContext(methodDeclaration, null, null, null, null, cToken);
            var factoryMethodAnalyzer = new FactoryMethodAnalyzer.OverridesAbstractFactoryMethod(context);

            //Act
            var result = factoryMethodAnalyzer.Analyze();

            //Assert
            Assert.False(result);
        }

        [Fact]
        public void Test_OverridesAbstractFactoryMethod_ShouldSucceed()
        {
            //Arrange
            var correctCode = @"
                public class ConcreteCreator1 : Creator
                {
                    public override IProduct FactoryMethod()
                    {
                        return new ConcreteProduct1();
                    }
                }";
            var syntaxTree = SyntaxFactory.ParseSyntaxTree(correctCode).GetRoot();
            var methodDeclaration = syntaxTree.DescendantNodes().OfType<MethodDeclarationSyntax>().FirstOrDefault();
            var cToken = new CancellationToken();
            var context = new SyntaxNodeAnalysisContext(methodDeclaration, null, null, null, null, cToken);
            var factoryMethodAnalyzer = new FactoryMethodAnalyzer.OverridesAbstractFactoryMethod(context);

            //Act
            var result = factoryMethodAnalyzer.Analyze();

            //Assert
            Assert.True(result);
        }

        [Fact]
        public void Test_IsProductInterface_NotUsedAsFactoryMethodReturnType_ShouldReturnFalse()
        {
            //Arrange
            var iProduct = @"
                public interface IProduct
                {
                    string Operation();
                }";
            var creatorClass = @"
                public abstract class Creator
                {
                    private abstract string FactoryMethod();
                }";
            var interfaceDeclaration = SyntaxFactory.ParseSyntaxTree(iProduct).GetRoot().DescendantNodes().OfType<InterfaceDeclarationSyntax>().FirstOrDefault();

            var workspace = new AdhocWorkspace()
                .AddSolution(SolutionInfo.Create(SolutionId.CreateNewId(), VersionStamp.Create(), null, null))
                .AddProject("DummyProject", "DummyProject", LanguageNames.CSharp)
                .AddMetadataReference(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
                .AddDocument("Creator.cs", creatorClass).Project.Solution;

            var dummyProject = workspace.Projects.FirstOrDefault();
            var cToken = new CancellationToken();
            var comp = dummyProject.GetCompilationAsync().Result;
            var sModel = comp.GetSemanticModel(comp.SyntaxTrees.First());
            var context = new SyntaxNodeAnalysisContext(interfaceDeclaration, sModel, null, null, null, cToken);
            var factoryMethodAnalyzer = new FactoryMethodAnalyzer.IsProductInterface(context);

            //Act
            var result = factoryMethodAnalyzer.Analyze();

            //Assert
            Assert.False(result);
        }

        [Fact]
        public void Test_IsProductInterface_ShouldSucceed()
        {        
            //Arrange
            var iProduct = @"
                public interface IProduct
                {
                    string Operation();
                }";
            var creatorClass = @"
                public abstract class Creator
                {
                    private abstract IProduct FactoryMethod();
                }";
            var interfaceDeclaration = SyntaxFactory.ParseSyntaxTree(iProduct).GetRoot().DescendantNodes().OfType<InterfaceDeclarationSyntax>().FirstOrDefault();

            var workspace = new AdhocWorkspace()
                .AddSolution(SolutionInfo.Create(SolutionId.CreateNewId(), VersionStamp.Create(), null, null))
                .AddProject("DummyProject", "DummyProject", LanguageNames.CSharp)
                .AddMetadataReference(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
                .AddDocument("Creator.cs", creatorClass).Project.Solution;

            var dummyProject = workspace.Projects.FirstOrDefault();
            var cToken = new CancellationToken();
            var comp = dummyProject.GetCompilationAsync().Result;
            var sModel = comp.GetSemanticModel(comp.SyntaxTrees.First());
            var context = new SyntaxNodeAnalysisContext(interfaceDeclaration, sModel, null, null, null, cToken);
            var factoryMethodAnalyzer = new FactoryMethodAnalyzer.IsProductInterface(context);

            //Act
            var result = factoryMethodAnalyzer.Analyze();

            //Assert
            Assert.True(result);
        }

        [Fact]
        public void Test_IsConcreteProduct_DoesNotImplementProductInterface_ShouldReturnFalse()
        {
            //Arrange
            var iProduct = @"
                public interface IProduct
                {
                    string Operation();
                }";
            var creatorClass = @"
                public abstract class Creator
                {
                    private abstract IProduct FactoryMethod();
                }";
            var concreteProduct = @"
                public class ConcreteProduct1
                {
                    public string Operation()
                    {
                        return ""{Result of ConcreteProduct1}"";
                    }
                }";
            var classDeclaration = SyntaxFactory.ParseSyntaxTree(concreteProduct).GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault();

            var workspace = new AdhocWorkspace()
                .AddSolution(SolutionInfo.Create(SolutionId.CreateNewId(), VersionStamp.Create(), null, null))
                .AddProject("DummyProject", "DummyProject", LanguageNames.CSharp)
                .AddMetadataReference(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
                .AddDocument("IProduct.cs", iProduct).Project.Solution;
            workspace = workspace.Projects.FirstOrDefault().AddDocument("Creator", creatorClass).Project.Solution;

            var dummyProject = workspace.Projects.FirstOrDefault();
            var cToken = new CancellationToken();
            var comp = dummyProject.GetCompilationAsync().Result;
            var sModel = comp.GetSemanticModel(comp.SyntaxTrees.First());
            var context = new SyntaxNodeAnalysisContext(classDeclaration, sModel, null, null, null, cToken);
            var factoryMethodAnalyzer = new FactoryMethodAnalyzer.IsConcreteProduct(context);

            //Act
            var result = factoryMethodAnalyzer.Analyze();

            //Assert
            Assert.False(result);
        }

        [Fact]
        public void Test_IsConcreteProduct_ProductInterfaceNotUsedAsReturnType_ShouldReturnFalse()
        {
            //Arrange
            var iProduct = @"
                public interface IProduct
                {
                    string Operation();
                }";
            var creatorClass = @"
                public abstract class Creator
                {
                    private abstract string FactoryMethod();
                }";
            var concreteProduct = @"
                public class ConcreteProduct1
                {
                    public string Operation()
                    {
                        return ""{Result of ConcreteProduct1}"";
                    }
                }";
            var classDeclaration = SyntaxFactory.ParseSyntaxTree(concreteProduct).GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault();

            var workspace = new AdhocWorkspace()
                .AddSolution(SolutionInfo.Create(SolutionId.CreateNewId(), VersionStamp.Create(), null, null))
                .AddProject("DummyProject", "DummyProject", LanguageNames.CSharp)
                .AddMetadataReference(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
                .AddDocument("IProduct.cs", iProduct).Project.Solution;
            workspace = workspace.Projects.FirstOrDefault().AddDocument("Creator", creatorClass).Project.Solution;

            var dummyProject = workspace.Projects.FirstOrDefault();
            var cToken = new CancellationToken();
            var comp = dummyProject.GetCompilationAsync().Result;
            var sModel = comp.GetSemanticModel(comp.SyntaxTrees.First());
            var context = new SyntaxNodeAnalysisContext(classDeclaration, sModel, null, null, null, cToken);
            var factoryMethodAnalyzer = new FactoryMethodAnalyzer.IsConcreteProduct(context);

            //Act
            var result = factoryMethodAnalyzer.Analyze();

            //Assert
            Assert.False(result);
        }

        [Fact]
        public void Test_IsConcreteProduct_ShouldSucceed()
        {
            //Arrange
            var iProduct = @"
                public interface IProduct
                {
                    string Operation();
                }";
            var creatorClass = @"
                public abstract class Creator
                {
                    private abstract IProduct FactoryMethod();
                }";
            var concreteProduct = @"
                public class ConcreteProduct1 : IProduct
                {
                    public string Operation()
                    {
                        return ""{Result of ConcreteProduct1}"";
                    }
                }";
            var classDeclaration = SyntaxFactory.ParseSyntaxTree(concreteProduct).GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault();

            var workspace = new AdhocWorkspace()
                .AddSolution(SolutionInfo.Create(SolutionId.CreateNewId(), VersionStamp.Create(), null, null))
                .AddProject("DummyProject", "DummyProject", LanguageNames.CSharp)
                .AddMetadataReference(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
                .AddDocument("IProduct.cs", iProduct).Project.Solution;
            workspace = workspace.Projects.FirstOrDefault().AddDocument("Creator", creatorClass).Project.Solution;

            var dummyProject = workspace.Projects.FirstOrDefault();
            var cToken = new CancellationToken();
            var comp = dummyProject.GetCompilationAsync().Result;
            var sModel = comp.GetSemanticModel(comp.SyntaxTrees.First());
            var context = new SyntaxNodeAnalysisContext(classDeclaration, sModel, null, null, null, cToken);
            var factoryMethodAnalyzer = new FactoryMethodAnalyzer.IsConcreteProduct(context);

            //Act
            var result = factoryMethodAnalyzer.Analyze();

            //Assert
            Assert.True(result);
        }

        [Fact]
        public void Test_DeclarationWithFactoryMethod_MethodReturnsPredefinedObject_ShouldReturnFalse()
        {
            //Arrange
            var creatorClass = @"
                public abstract class Creator
                {
                    private abstract string FactoryMethod();

                    public string SomeOperation()
                    {
                        // Call the factory method to create a Product object.
                        var product = FactoryMethod();
                    }
                }";
            var localDeclaration = SyntaxFactory.ParseSyntaxTree(creatorClass).GetRoot().DescendantNodes().OfType<LocalDeclarationStatementSyntax>().FirstOrDefault();

            var workspace = new AdhocWorkspace()
                .AddSolution(SolutionInfo.Create(SolutionId.CreateNewId(), VersionStamp.Create(), null, null))
                .AddProject("DummyProject", "DummyProject", LanguageNames.CSharp)
                .AddMetadataReference(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
                .AddDocument("Creator.cs", creatorClass).Project.Solution;

            var dummyProject = workspace.Projects.FirstOrDefault();
            var cToken = new CancellationToken();
            var comp = dummyProject.GetCompilationAsync().Result;
            var sModel = comp.GetSemanticModel(comp.SyntaxTrees.First());
            var context = new SyntaxNodeAnalysisContext(localDeclaration, sModel, null, null, null, cToken);
            var factoryMethodAnalyzer = new FactoryMethodAnalyzer.DeclarationWithFactoryMethod(context);

            //Act
            var result = factoryMethodAnalyzer.Analyze();

            //Assert
            Assert.False(result);
        }

        [Fact]
        public void Test_DeclarationWithFactoryMethod_ShouldSucceed()
        {
            //Arrange
            var creatorClass = @"
                public abstract class Creator
                {
                    private abstract IProduct FactoryMethod();

                    public string SomeOperation()
                    {
                        // Call the factory method to create a Product object.
                        var product = FactoryMethod();
                    }
                }";
            var localDeclaration = SyntaxFactory.ParseSyntaxTree(creatorClass).GetRoot().DescendantNodes().OfType<LocalDeclarationStatementSyntax>().FirstOrDefault();

            var workspace = new AdhocWorkspace()
                .AddSolution(SolutionInfo.Create(SolutionId.CreateNewId(), VersionStamp.Create(), null, null))
                .AddProject("DummyProject", "DummyProject", LanguageNames.CSharp)
                .AddMetadataReference(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
                .AddDocument("Creator.cs", creatorClass).Project.Solution;

            var dummyProject = workspace.Projects.FirstOrDefault();
            var cToken = new CancellationToken();
            var comp = dummyProject.GetCompilationAsync().Result;
            var sModel = comp.GetSemanticModel(comp.SyntaxTrees.First());
            var context = new SyntaxNodeAnalysisContext(localDeclaration, sModel, null, null, null, cToken);
            var factoryMethodAnalyzer = new FactoryMethodAnalyzer.DeclarationWithFactoryMethod(context);

            //Act
            var result = factoryMethodAnalyzer.Analyze();

            //Assert
            Assert.True(result);
        }
    }
}
