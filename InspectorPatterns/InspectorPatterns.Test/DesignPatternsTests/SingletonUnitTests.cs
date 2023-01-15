using Microsoft.CodeAnalysis.CSharp;
using Xunit;
using InspectorPatterns.Core;
using InspectorPatterns.Core.Analyzers;

namespace InspectorPatterns.Test.DesignPatternsTests
{
    public class SingletonUnitTests
    {
        [Fact]
        public void Test_HasGetInstanceSelfMethod_NoMethods()
        {
            //Arrange
            var wrongCode = @"
                public sealed class Singleton
                {
                    private static Singleton _instance;

                    private Singleton() { }
                }";
            var contextNode = SyntaxFactory.ParseSyntaxTree(wrongCode).GetRoot();
            var singletonAnalyzer = new SingletonAnalyzer.HasGetInstanceSelfMethod(contextNode);

            //Act
            var result = singletonAnalyzer.Analyze();

            //Assert
            Assert.False(result);
        }

        [Fact]
        public void Test_HasGetInstanceSelfMethod_NoMethodsWithClassReturnType()
        {
            //Arrange
            var wrongCode = @"
                public sealed class Singleton
                {
                    private static Singleton _instance;

                    private Singleton() { }

                    private void Check()
                    {
                        Console.WriteLine(_teststring);
                    }
                }";
            var contextNode = SyntaxFactory.ParseSyntaxTree(wrongCode).GetRoot();
            var singletonAnalyzer = new SingletonAnalyzer.HasGetInstanceSelfMethod(contextNode);

            //Act
            var result = singletonAnalyzer.Analyze();

            //Assert
            Assert.False(result);
        }

        [Fact]
        public void Test_HasGetInstanceSelfMethod_ShouldSucceed()
        {
            //Arrange
            var correctCode = @"
                public sealed class Singleton
                {
                    private static Singleton _instance;

                    private Singleton() { }

                    public static string HasNotReturnTypeSelf()
                    {
                        return string.Empty;
                    }

                    public static Singleton GetInstance()
                    {
                        return _instance;
                    }
                }";
            var contextNode = SyntaxFactory.ParseSyntaxTree(correctCode).GetRoot();
            var singletonAnalyzer = new SingletonAnalyzer.HasGetInstanceSelfMethod(contextNode);

            //Act
            var result = singletonAnalyzer.Analyze();

            //Assert
            Assert.True(result);
        }

        [Fact]
        public void Test_HasPrivateConstructor_NoConstructors()
        {
            //Arrange
            var wrongCode = @"
                public sealed class Singleton
                {
                    private static Singleton _instance;

                    public static Singleton GetInstance()
                    {
                        return _instance;
                    }
                }";
            var contextNode = SyntaxFactory.ParseSyntaxTree(wrongCode).GetRoot();
            var singletonAnalyzer = new SingletonAnalyzer.HasPrivateConstructor(contextNode);

            //Act
            var result = singletonAnalyzer.Analyze();

            //Assert
            Assert.False(result);
        }

        [Fact]
        public void Test_HasPrivateConstructor_NoPrivateConstructors()
        {
            //Arrange
            var wrongCode = @"
                public sealed class Singleton
                {
                    private static Singleton _instance;

                    public Singleton() { }

                    public static Singleton GetInstance()
                    {
                        return _instance;
                    }
                }";
            var contextNode = SyntaxFactory.ParseSyntaxTree(wrongCode).GetRoot();
            var singletonAnalyzer = new SingletonAnalyzer.HasPrivateConstructor(contextNode);

            //Act
            var result = singletonAnalyzer.Analyze();

            //Assert
            Assert.False(result);
        }

        [Fact]
        public void Test_HasPrivateConstructor_ShouldSucceed()
        {
            //Arrange
            var correctCode = @"
                public sealed class Singleton
                {
                    private static Singleton _instance;

                    private Singleton() { }

                    public static Singleton GetInstance()
                    {
                        return _instance;
                    }
                }";
            var contextNode = SyntaxFactory.ParseSyntaxTree(correctCode).GetRoot();
            var singletonAnalyzer = new SingletonAnalyzer.HasPrivateConstructor(contextNode);

            //Act
            var result = singletonAnalyzer.Analyze();

            //Assert
            Assert.True(result);
        }

        [Fact]
        public void Test_HasPrivateStaticSelfField_NoFields()
        {
            //Arrange
            var wrongCode = @"
                public sealed class Singleton
                {
                    private Singleton() { }

                    private void Check()
                    {
                        Console.WriteLine();
                    }
                }";
            var contextNode = SyntaxFactory.ParseSyntaxTree(wrongCode).GetRoot();
            var singletonAnalyzer = new SingletonAnalyzer.HasPrivateStaticSelfField(contextNode);

            //Act
            var result = singletonAnalyzer.Analyze();

            //Assert
            Assert.False(result);
        }

        [Fact]
        public void Test_HasPrivateStaticSelfField_NoPrivateFields()
        {
            //Arrange
            var wrongCode = @"
                public sealed class Singleton
                {
                    public static Singleton _instance;

                    private Singleton() { }

                    public static Singleton GetInstance()
                    {
                        return _instance;
                    }
                }";
            var contextNode = SyntaxFactory.ParseSyntaxTree(wrongCode).GetRoot();
            var singletonAnalyzer = new SingletonAnalyzer.HasPrivateStaticSelfField(contextNode);

            //Act
            var result = singletonAnalyzer.Analyze();

            //Assert
            Assert.False(result);
        }

        [Fact]
        public void Test_HasPrivateStaticSelfField_NoStaticFields()
        {
            //Arrange
            var wrongCode = @"
                public sealed class Singleton
                {
                    private Singleton _instance;

                    private Singleton() { }

                    public static Singleton GetInstance()
                    {
                        return _instance;
                    }
                }";
            var contextNode = SyntaxFactory.ParseSyntaxTree(wrongCode).GetRoot();
            var singletonAnalyzer = new SingletonAnalyzer.HasPrivateStaticSelfField(contextNode);

            //Act
            var result = singletonAnalyzer.Analyze();

            //Assert
            Assert.False(result);
        }

        [Fact]
        public void Test_HasPrivateStaticSelfField_NoReturnTypeClassFields()
        {
            //Arrange
            var wrongCode = @"
                public sealed class Singleton
                {
                    private static string _teststring;

                    private Singleton() { }

                    private void Check()
                    {
                        Console.WriteLine(_teststring);
                    }
                }";
            var contextNode = SyntaxFactory.ParseSyntaxTree(wrongCode).GetRoot();
            var singletonAnalyzer = new SingletonAnalyzer.HasPrivateStaticSelfField(contextNode);

            //Act
            var result = singletonAnalyzer.Analyze();

            //Assert
            Assert.False(result);
        }

        [Fact]
        public void Test_HasPrivateStaticSelfField_ShouldSucceed()
        {
            //Arrange
            var correctCode = @"
                public sealed class Singleton
                {
                    private string _testIncorrectType;
                    private static Singleton _instance;

                    private Singleton() { }
                    private Singleton(string) { }

                    public static Singleton GetInstance()
                    {
                        return _instance;
                    }

                    private void check()
                    {
                        Console.WriteLine(_testIncorrectType);
                    }
                }";
            var contextNode = SyntaxFactory.ParseSyntaxTree(correctCode).GetRoot();
            var singletonAnalyzer = new SingletonAnalyzer.HasPrivateStaticSelfField(contextNode);

            //Act
            var result = singletonAnalyzer.Analyze();

            //Assert
            Assert.True(result);
        }

        [Fact]
        public void Test_Analyze_ShouldFail()
        {
            //Arrange
            var correctCode = @"
                public sealed class Singleton
                {
                    private static Singleton _instance;

                    public static Singleton GetInstance()
                    {
                        return _instance;
                    }
                }";
            var contextNode = SyntaxFactory.ParseSyntaxTree(correctCode).GetRoot();
            var designPatternAnalyzer = new DesignPatternAnalyzer();

            //Act
            designPatternAnalyzer.SetAnalyzerStrategy(new SingletonAnalyzer.HasGetInstanceSelfMethod(contextNode));
            var hasGetInstanceSelf = designPatternAnalyzer.Analyze();

            designPatternAnalyzer.SetAnalyzerStrategy(new SingletonAnalyzer.HasPrivateConstructor(contextNode));
            var hasPrivateConstructor = designPatternAnalyzer.Analyze();

            designPatternAnalyzer.SetAnalyzerStrategy(new SingletonAnalyzer.HasPrivateStaticSelfField(contextNode));
            var hasPrivateStaticSelfField = designPatternAnalyzer.Analyze();

            //Assert
            Assert.False(hasGetInstanceSelf && hasPrivateConstructor && hasPrivateStaticSelfField);
        }

        [Fact]
        public void Test_Analyze_ShouldSucceed()
        {
            //Arrange
            var correctCode = @"
                public sealed class Singleton
                {
                    private static Singleton _instance;
                    private string _teststring;

                    private Singleton() { }
                    private Singleton(string) { }

                    public static Singleton GetInstance()
                    {
                        return _instance;
                    }

                    private void check()
                    {
                        Console.WriteLine(_teststring);
                    }
                }";
            var contextNode = SyntaxFactory.ParseSyntaxTree(correctCode).GetRoot();
            var designPatternAnalyzer = new DesignPatternAnalyzer();

            //Act
            designPatternAnalyzer.SetAnalyzerStrategy(new SingletonAnalyzer.HasGetInstanceSelfMethod(contextNode));
            var hasGetInstanceSelf = designPatternAnalyzer.Analyze();

            designPatternAnalyzer.SetAnalyzerStrategy(new SingletonAnalyzer.HasPrivateConstructor(contextNode));
            var hasPrivateConstructor = designPatternAnalyzer.Analyze();

            designPatternAnalyzer.SetAnalyzerStrategy(new SingletonAnalyzer.HasPrivateStaticSelfField(contextNode));
            var hasPrivateStaticSelfField = designPatternAnalyzer.Analyze();

            //Assert
            Assert.True(hasGetInstanceSelf && hasPrivateConstructor && hasPrivateStaticSelfField);
        }
    }
}
