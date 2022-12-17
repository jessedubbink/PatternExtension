using Microsoft.CodeAnalysis.CSharp;
using InspectorPatterns.Core.DesignPatterns.Analyzers;
using Xunit;

namespace InspectorPatterns.Test.DesignPatternsTests
{
    public class SingletonUnitTests
    {
        private SingletonAnalyzer _singletonAnalyzer;

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
            _singletonAnalyzer = new SingletonAnalyzer(contextNode);

            //Act
            var result = _singletonAnalyzer.HasGetInstanceSelfMethod();

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
            _singletonAnalyzer = new SingletonAnalyzer(contextNode);

            //Act
            var result = _singletonAnalyzer.HasGetInstanceSelfMethod();

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

                    public static Singleton GetInstance()
                    {
                        return _instance;
                    }
                }";
            var contextNode = SyntaxFactory.ParseSyntaxTree(correctCode).GetRoot();
            _singletonAnalyzer = new SingletonAnalyzer(contextNode);

            //Act
            var result = _singletonAnalyzer.HasGetInstanceSelfMethod();

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
            _singletonAnalyzer = new SingletonAnalyzer(contextNode);

            //Act
            var result = _singletonAnalyzer.HasPrivateConstructor();

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
            _singletonAnalyzer = new SingletonAnalyzer(contextNode);

            //Act
            var result = _singletonAnalyzer.HasPrivateConstructor();

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
            _singletonAnalyzer = new SingletonAnalyzer(contextNode);

            //Act
            var result = _singletonAnalyzer.HasPrivateConstructor();

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
            _singletonAnalyzer = new SingletonAnalyzer(contextNode);

            //Act
            var result = _singletonAnalyzer.HasPrivateStaticSelfField();

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
            _singletonAnalyzer = new SingletonAnalyzer(contextNode);

            //Act
            var result = _singletonAnalyzer.HasPrivateStaticSelfField();

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
            _singletonAnalyzer = new SingletonAnalyzer(contextNode);

            //Act
            var result = _singletonAnalyzer.HasPrivateStaticSelfField();

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
            _singletonAnalyzer = new SingletonAnalyzer(contextNode);

            //Act
            var result = _singletonAnalyzer.HasPrivateStaticSelfField();

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
            _singletonAnalyzer = new SingletonAnalyzer(contextNode);

            //Act
            var result = _singletonAnalyzer.HasPrivateStaticSelfField();

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
            _singletonAnalyzer = new SingletonAnalyzer(contextNode);

            //Act
            var result = _singletonAnalyzer.Analyze();

            //Assert
            Assert.False(result);
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
            _singletonAnalyzer = new SingletonAnalyzer(contextNode);

            //Act
            var result = _singletonAnalyzer.Analyze();
            var resultLocation = _singletonAnalyzer.GetLocation();

            //Assert
            Assert.True(result);
            Assert.NotNull(resultLocation);
        }

        // Test HasGetInstanceSelfMethod()
        // - Error/ Classtree has no methods
        // - Error/ Methods return type is not Class type
        // - Succes/ Method exists and has return type of class
        // Test HasPrivateConstructor()
        // - Error/ Classtree has no constructors
        // - Error/ Constructor is not private
        // - Succes/ Constructor exists and is private
        // Test HasPrivateStaticSelfField()
        // - Error/ Classtree has no fields
        // - Error/ Field is not private
        // - Error/ Field is not static
        // - Error/ Field type is not class
        // - Succes/ Field exists is private and static and has type of class
        // Test Analyze()
        // - Error/ When one check is false
        // - Succes/ When all checks are true
        //   * Check if location is set

        // Gezochte situatie
        // Ik vul een string in dat een class moet representeren
        // De string wordt omgezet naar een visual studio syntaxtree
        // De syntaxtree wordt omgezet naar onze datamodel
        // Met dat datamodel kan ik een niewe design pattern aanmaken
        // Ik kan dan alle verschillende functionaliteit van de design pattern checken
    }
}
