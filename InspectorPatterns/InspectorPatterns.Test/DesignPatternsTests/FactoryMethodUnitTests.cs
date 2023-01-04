using Microsoft.CodeAnalysis.CSharp;
using Xunit;
using InspectorPatterns.Core.Analyzers;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Threading;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis;

namespace InspectorPatterns.Test.DesignPatternsTests
{
    public class FactoryMethodUnitTests
    {
        [Fact]
        public void Test_IsAbstractFactoryMethod_NoAbstractMethods()
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
        public void Test_IsAbstractFactoryMethod_NoMethodsWithNonePredefinedReturnType()
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
        public void Test_OverridesAbstractFactoryMethod_NoMethodsWithNonePredefinedReturnType()
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
        public void Test_OverridesAbstractFactoryMethod_NoMethodsWithOverrideModifier()
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
        public void Test_OverridesAbstractFactoryMethod_MethodBodyDoesNotContainNewExpression()
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
        public void Test_IsProductInterface_ShouldSucceed()
        {
            //Arrange
            var iProduct = @"
                public interface IProduct
                {
                    string Operation();
                }";
            var concreteProduct = @"
                public abstract class Creator
                {
                    private abstract IProduct FactoryMethod();
                }";
            var syntaxTree = SyntaxFactory.ParseSyntaxTree(iProduct).GetRoot();
            var interfaceDeclaration = syntaxTree.DescendantNodes().OfType<InterfaceDeclarationSyntax>().FirstOrDefault();
            //var compilation = SyntaxFactory.ParseCompilationUnit(concreteProduct);
            
            var workspace = MSBuildWorkspace.Create();
            var sln = workspace.CurrentSolution;
            var dummyProject = sln.AddProject("DummyProject", "DummyProject", "C#");
            var doc = dummyProject.AddDocument("ConcreteProduct.cs", concreteProduct);
            var cToken = new CancellationToken();
            var comp = dummyProject.GetCompilationAsync().Result;
            var sModel = comp.GetSemanticModel(comp.SyntaxTrees.First());
            var context = new SyntaxNodeAnalysisContext(interfaceDeclaration, sModel, null, null, null, cToken);
            var factoryMethodAnalyzer = new FactoryMethodAnalyzer.OverridesAbstractFactoryMethod(context);

            //Act
            var result = factoryMethodAnalyzer.Analyze();

            //Assert
            Assert.True(result);
        }

        //[Fact]
        //public void Test_HasPrivateConstructor_ShouldSucceed()
        //{
        //    //Arrange
        //    var correctCode = @"
        //        public sealed class Singleton
        //        {
        //            private static Singleton _instance;

        //            private Singleton() { }

        //            public static Singleton GetInstance()
        //            {
        //                return _instance;
        //            }
        //        }";
        //    var contextNode = SyntaxFactory.ParseSyntaxTree(correctCode).GetRoot();
        //    var factoryMethodAnalyzer = new SingletonAnalyzer.HasPrivateConstructor(contextNode);

        //    //Act
        //    var result = factoryMethodAnalyzer.Analyze();

        //    //Assert
        //    Assert.True(result);
        //}

        //[Fact]
        //public void Test_HasPrivateStaticSelfField_NoFields()
        //{
        //    //Arrange
        //    var wrongCode = @"
        //        public sealed class Singleton
        //        {
        //            private Singleton() { }

        //            private void Check()
        //            {
        //                Console.WriteLine();
        //            }
        //        }";
        //    var contextNode = SyntaxFactory.ParseSyntaxTree(wrongCode).GetRoot();
        //    var factoryMethodAnalyzer = new SingletonAnalyzer.HasPrivateStaticSelfField(contextNode);

        //    //Act
        //    var result = factoryMethodAnalyzer.Analyze();

        //    //Assert
        //    Assert.False(result);
        //}

        //[Fact]
        //public void Test_HasPrivateStaticSelfField_NoPrivateFields()
        //{
        //    //Arrange
        //    var wrongCode = @"
        //        public sealed class Singleton
        //        {
        //            public static Singleton _instance;

        //            private Singleton() { }

        //            public static Singleton GetInstance()
        //            {
        //                return _instance;
        //            }
        //        }";
        //    var contextNode = SyntaxFactory.ParseSyntaxTree(wrongCode).GetRoot();
        //    var factoryMethodAnalyzer = new SingletonAnalyzer.HasPrivateStaticSelfField(contextNode);

        //    //Act
        //    var result = factoryMethodAnalyzer.Analyze();

        //    //Assert
        //    Assert.False(result);
        //}

        //[Fact]
        //public void Test_HasPrivateStaticSelfField_NoStaticFields()
        //{
        //    //Arrange
        //    var wrongCode = @"
        //        public sealed class Singleton
        //        {
        //            private Singleton _instance;

        //            private Singleton() { }

        //            public static Singleton GetInstance()
        //            {
        //                return _instance;
        //            }
        //        }";
        //    var contextNode = SyntaxFactory.ParseSyntaxTree(wrongCode).GetRoot();
        //    var factoryMethodAnalyzer = new SingletonAnalyzer.HasPrivateStaticSelfField(contextNode);

        //    //Act
        //    var result = factoryMethodAnalyzer.Analyze();

        //    //Assert
        //    Assert.False(result);
        //}

        //[Fact]
        //public void Test_HasPrivateStaticSelfField_NoReturnTypeClassFields()
        //{
        //    //Arrange
        //    var wrongCode = @"
        //        public sealed class Singleton
        //        {
        //            private static string _teststring;

        //            private Singleton() { }

        //            private void Check()
        //            {
        //                Console.WriteLine(_teststring);
        //            }
        //        }";
        //    var contextNode = SyntaxFactory.ParseSyntaxTree(wrongCode).GetRoot();
        //    var factoryMethodAnalyzer = new SingletonAnalyzer.HasPrivateStaticSelfField(contextNode);

        //    //Act
        //    var result = factoryMethodAnalyzer.Analyze();

        //    //Assert
        //    Assert.False(result);
        //}

        //[Fact]
        //public void Test_HasPrivateStaticSelfField_ShouldSucceed()
        //{
        //    //Arrange
        //    var correctCode = @"
        //        public sealed class Singleton
        //        {
        //            private string _testIncorrectType;
        //            private static Singleton _instance;

        //            private Singleton() { }
        //            private Singleton(string) { }

        //            public static Singleton GetInstance()
        //            {
        //                return _instance;
        //            }

        //            private void check()
        //            {
        //                Console.WriteLine(_testIncorrectType);
        //            }
        //        }";
        //    var contextNode = SyntaxFactory.ParseSyntaxTree(correctCode).GetRoot();
        //    var factoryMethodAnalyzer = new SingletonAnalyzer.HasPrivateStaticSelfField(contextNode);

        //    //Act
        //    var result = factoryMethodAnalyzer.Analyze();

        //    //Assert
        //    Assert.True(result);
        //}

        //[Fact]
        //public void Test_Analyze_ShouldFail()
        //{
        //    //Arrange
        //    var correctCode = @"
        //        public sealed class Singleton
        //        {
        //            private static Singleton _instance;

        //            public static Singleton GetInstance()
        //            {
        //                return _instance;
        //            }
        //        }";
        //    var contextNode = SyntaxFactory.ParseSyntaxTree(correctCode).GetRoot();
        //    var designPatternAnalyzer = new DesignPatternAnalyzer();

        //    //Act
        //    designPatternAnalyzer.SetAnalyzerStrategy(new SingletonAnalyzer.HasGetInstanceSelfMethod(contextNode));
        //    var hasGetInstanceSelf = designPatternAnalyzer.Analyze();

        //    designPatternAnalyzer.SetAnalyzerStrategy(new SingletonAnalyzer.HasPrivateConstructor(contextNode));
        //    var hasPrivateConstructor = designPatternAnalyzer.Analyze();

        //    designPatternAnalyzer.SetAnalyzerStrategy(new SingletonAnalyzer.HasPrivateStaticSelfField(contextNode));
        //    var hasPrivateStaticSelfField = designPatternAnalyzer.Analyze();

        //    //Assert
        //    Assert.False(hasGetInstanceSelf && hasPrivateConstructor && hasPrivateStaticSelfField);
        //}

        //[Fact]
        //public void Test_Analyze_ShouldSucceed()
        //{
        //    //Arrange
        //    var correctCode = @"
        //        public sealed class Singleton
        //        {
        //            private static Singleton _instance;
        //            private string _teststring;

        //            private Singleton() { }
        //            private Singleton(string) { }

        //            public static Singleton GetInstance()
        //            {
        //                return _instance;
        //            }

        //            private void check()
        //            {
        //                Console.WriteLine(_teststring);
        //            }
        //        }";
        //    var contextNode = SyntaxFactory.ParseSyntaxTree(correctCode).GetRoot();
        //    var designPatternAnalyzer = new DesignPatternAnalyzer();

        //    //Act
        //    designPatternAnalyzer.SetAnalyzerStrategy(new SingletonAnalyzer.HasGetInstanceSelfMethod(contextNode));
        //    var hasGetInstanceSelf = designPatternAnalyzer.Analyze();

        //    designPatternAnalyzer.SetAnalyzerStrategy(new SingletonAnalyzer.HasPrivateConstructor(contextNode));
        //    var hasPrivateConstructor = designPatternAnalyzer.Analyze();

        //    designPatternAnalyzer.SetAnalyzerStrategy(new SingletonAnalyzer.HasPrivateStaticSelfField(contextNode));
        //    var hasPrivateStaticSelfField = designPatternAnalyzer.Analyze();

        //    //Assert
        //    Assert.True(hasGetInstanceSelf && hasPrivateConstructor && hasPrivateStaticSelfField);
        //}
    }
}
