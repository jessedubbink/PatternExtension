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
    public class DecoratorUnitTests
    {
        [Fact]
        public void Test_AnalyzeStatement_ShouldReturnTrue()
        {
            //Arrange
            var decoratorClass = @"
                public abstract class Decorator
                {
		            public Test()
		            {
			            Beverage beverage = new Milk(new Soy());
		            }
                }";
            var contextNode = SyntaxFactory.ParseSyntaxTree(decoratorClass).GetRoot();
            var localDeclaration = contextNode.DescendantNodes().OfType<LocalDeclarationStatementSyntax>().FirstOrDefault();

            var workspace = new AdhocWorkspace()
                .AddSolution(SolutionInfo.Create(SolutionId.CreateNewId(), VersionStamp.Create(), null, null))
                .AddProject("DummyProject", "DummyProject", LanguageNames.CSharp)
                .AddMetadataReference(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
                .AddDocument("Decorator.cs", decoratorClass).Project.Solution;


            var dummyProject = workspace.Projects.FirstOrDefault();
            var cToken = new CancellationToken();
            var comp = dummyProject.GetCompilationAsync().Result;
            var sModel = comp.GetSemanticModel(comp.SyntaxTrees.First());
            var context = new SyntaxNodeAnalysisContext(localDeclaration, sModel, null, null, null, cToken);

            //Assert
            Assert.True((new DecoratorAnalyzer.AnalyzeStatement(context)).Analyze());
        }

        [Fact]
        public void Test_AnalyzeStatement_ShouldReturnFalse()
        {
            //Arrange
            var decoratorClass = @"
                public abstract class Decorator
                {
		            public Test()
		            {
			            Beverage beverage = new Milk(new Soy('test'));
		            }
                }";
            var contextNode = SyntaxFactory.ParseSyntaxTree(decoratorClass).GetRoot();
            var localDeclaration = contextNode.DescendantNodes().OfType<LocalDeclarationStatementSyntax>().FirstOrDefault();

            var workspace = new AdhocWorkspace()
                .AddSolution(SolutionInfo.Create(SolutionId.CreateNewId(), VersionStamp.Create(), null, null))
                .AddProject("DummyProject", "DummyProject", LanguageNames.CSharp)
                .AddMetadataReference(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
                .AddDocument("Decorator.cs", decoratorClass).Project.Solution;


            var dummyProject = workspace.Projects.FirstOrDefault();
            var cToken = new CancellationToken();
            var comp = dummyProject.GetCompilationAsync().Result;
            var sModel = comp.GetSemanticModel(comp.SyntaxTrees.First());
            var context = new SyntaxNodeAnalysisContext(localDeclaration, sModel, null, null, null, cToken);

            //Assert
            Assert.False((new DecoratorAnalyzer.AnalyzeStatement(context)).Analyze());
        }

        [Fact]
        public void Test_HasCorrectConstructor_ShouldSucceed()
        {
            //Arrange
            var correctCode = @"
                    public class Milk: Beverage
                    {
                        private Beverage baseBeverage;

                        public Milk(Beverage beverage)
                        {
                            this.baseBeverage = beverage;
                        }

                        public override String Description
                        {
                            get
                            {
                                return baseBeverage.Description + "", Melk"";
                            }
                        }

                        public override double Cost()
                        {
                            return 0.35 + baseBeverage.Cost();
                        }
                    }

                    abstract class Beverage {
                        
                    }";

            var contextNode = SyntaxFactory.ParseSyntaxTree(correctCode).GetRoot();

            var workspace = new AdhocWorkspace()
                .AddSolution(SolutionInfo.Create(SolutionId.CreateNewId(), VersionStamp.Create(), null, null))
                .AddProject("DummyProject", "DummyProject", LanguageNames.CSharp)
                .AddMetadataReference(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
                .AddDocument("Decorator.cs", correctCode).Project.Solution;


            var dummyProject = workspace.Projects.FirstOrDefault();
            var cToken = new CancellationToken();
            var comp = dummyProject.GetCompilationAsync().Result;
            var sModel = comp.GetSemanticModel(comp.SyntaxTrees.First());
            var abstractClass = new SyntaxNodeAnalysisContext(contextNode.DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault(), sModel, null, null, null, cToken);

            var hasAbatractClass = (new DecoratorAnalyzer.HasAbstractClass(abstractClass)).Analyze();
            var staticField = (new DecoratorAnalyzer.HasPrivateStaticField(contextNode)).Analyze();
            var decoratorAnalyzer = (new DecoratorAnalyzer.HasConstructor(contextNode)).Analyze();

            //Assert
            Assert.True(hasAbatractClass);
            Assert.True((DecoratorAnalyzer._identifierValue == "Beverage"));
            Assert.True(staticField);
            Assert.True(decoratorAnalyzer);
        }

        [Fact]
        public void Test_HasCorrectConstructor_ShouldFail()
        {
            //Arrange
            var correctCode = @"
                    public class Milk: Beverage
                    {
                        private string baseBeverage;

                        public Milk(string beverage)
                        {
                            this.baseBeverage = beverage;
                        }

                        public override String Description
                        {
                            get
                            {
                                return baseBeverage.Description + "", Melk"";
                            }
                        }

                        public override double Cost()
                        {
                            return 0.35 + baseBeverage.Cost();
                        }
                    }

                    abstract class Beverage {
                        
                    }";

            var contextNode = SyntaxFactory.ParseSyntaxTree(correctCode).GetRoot();

            var workspace = new AdhocWorkspace()
                .AddSolution(SolutionInfo.Create(SolutionId.CreateNewId(), VersionStamp.Create(), null, null))
                .AddProject("DummyProject", "DummyProject", LanguageNames.CSharp)
                .AddMetadataReference(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
                .AddDocument("Decorator.cs", correctCode).Project.Solution;


            var dummyProject = workspace.Projects.FirstOrDefault();
            var cToken = new CancellationToken();
            var comp = dummyProject.GetCompilationAsync().Result;
            var sModel = comp.GetSemanticModel(comp.SyntaxTrees.First());
            var abstractClass = new SyntaxNodeAnalysisContext(contextNode.DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault(), sModel, null, null, null, cToken);

            var hasAbatractClass = (new DecoratorAnalyzer.HasAbstractClass(abstractClass)).Analyze();
            var staticField = (new DecoratorAnalyzer.HasPrivateStaticField(contextNode)).Analyze();
            var decoratorAnalyzer = (new DecoratorAnalyzer.HasConstructor(contextNode)).Analyze();

            //Assert
            Assert.True(hasAbatractClass);
            Assert.True((DecoratorAnalyzer._identifierValue == "Beverage"));
            Assert.False(staticField);
            Assert.False(decoratorAnalyzer);
        }

        [Fact]
        public void Test_HasPrivateStaticField_ShouldPublicFail()
        {
            //Arrange
            var correctCode = @"
                    public class Milk: Beverage
                    {
                        public Beverage baseBeverage;

                        public Milk(Beverage beverage)
                        {
                            this.baseBeverage = beverage;
                        }

                        public override String Description
                        {
                            get
                            {
                                return baseBeverage.Description + "", Melk"";
                            }
                        }

                        public override double Cost()
                        {
                            return 0.35 + baseBeverage.Cost();
                        }
                    }

                    abstract class Beverage {
                        
                    }";

            var contextNode = SyntaxFactory.ParseSyntaxTree(correctCode).GetRoot();

            var workspace = new AdhocWorkspace()
                .AddSolution(SolutionInfo.Create(SolutionId.CreateNewId(), VersionStamp.Create(), null, null))
                .AddProject("DummyProject", "DummyProject", LanguageNames.CSharp)
                .AddMetadataReference(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
                .AddDocument("Decorator.cs", correctCode).Project.Solution;


            var dummyProject = workspace.Projects.FirstOrDefault();
            var cToken = new CancellationToken();
            var comp = dummyProject.GetCompilationAsync().Result;
            var sModel = comp.GetSemanticModel(comp.SyntaxTrees.First());
            var abstractClass = new SyntaxNodeAnalysisContext(contextNode.DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault(), sModel, null, null, null, cToken);

            var hasAbatractClass = (new DecoratorAnalyzer.HasAbstractClass(abstractClass)).Analyze();
            var staticField = (new DecoratorAnalyzer.HasPrivateStaticField(contextNode)).Analyze();

            //Assert
            Assert.True(hasAbatractClass);
            Assert.True((DecoratorAnalyzer._identifierValue == "Beverage"));
            Assert.False(staticField);
        }

        [Fact]
        public void Test_HasPrivateStaticField_ShouldIncorrectFail()
        {
            //Arrange
            var correctCode = @"
                    public class Milk: Beverage
                    {
                        public string baseBeverage;

                        public Milk(string beverage)
                        {
                            this.baseBeverage = beverage;
                        }

                        public override String Description
                        {
                            get
                            {
                                return baseBeverage.Description + "", Melk"";
                            }
                        }

                        public override double Cost()
                        {
                            return 0.35 + baseBeverage.Cost();
                        }
                    }

                    abstract class Beverage {
                        
                    }";

            var contextNode = SyntaxFactory.ParseSyntaxTree(correctCode).GetRoot();

            var workspace = new AdhocWorkspace()
                .AddSolution(SolutionInfo.Create(SolutionId.CreateNewId(), VersionStamp.Create(), null, null))
                .AddProject("DummyProject", "DummyProject", LanguageNames.CSharp)
                .AddMetadataReference(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
                .AddDocument("Decorator.cs", correctCode).Project.Solution;


            var dummyProject = workspace.Projects.FirstOrDefault();
            var cToken = new CancellationToken();
            var comp = dummyProject.GetCompilationAsync().Result;
            var sModel = comp.GetSemanticModel(comp.SyntaxTrees.First());
            var abstractClass = new SyntaxNodeAnalysisContext(contextNode.DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault(), sModel, null, null, null, cToken);

            var hasAbatractClass = (new DecoratorAnalyzer.HasAbstractClass(abstractClass)).Analyze();
            var staticField = (new DecoratorAnalyzer.HasPrivateStaticField(contextNode)).Analyze();

            //Assert
            Assert.True(hasAbatractClass);
            Assert.True((DecoratorAnalyzer._identifierValue == "Beverage"));
            Assert.False(staticField);
        }

        [Fact]
        public void Test_IsDecorator()
        {
            //Arrange
            var correctCode = @"
                    public class Milk: Beverage
                    {
                        private Beverage baseBeverage;

                        public Milk(Beverage beverage)
                        {
                            this.baseBeverage = beverage;
                        }

                        public override String Description
                        {
                            get
                            {
                                return baseBeverage.Description + "", Melk"";
                            }
                        }

                        public override double Cost()
                        {
                            return 0.35 + baseBeverage.Cost();
                        }
                    }

                    abstract class Beverage {
                        
                    }";

            var contextNode = SyntaxFactory.ParseSyntaxTree(correctCode).GetRoot();

            var workspace = new AdhocWorkspace()
                .AddSolution(SolutionInfo.Create(SolutionId.CreateNewId(), VersionStamp.Create(), null, null))
                .AddProject("DummyProject", "DummyProject", LanguageNames.CSharp)
                .AddMetadataReference(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
                .AddDocument("Decorator.cs", correctCode).Project.Solution;


            var dummyProject = workspace.Projects.FirstOrDefault();
            var cToken = new CancellationToken();
            var comp = dummyProject.GetCompilationAsync().Result;
            var sModel = comp.GetSemanticModel(comp.SyntaxTrees.First());
            var abstractClass = new SyntaxNodeAnalysisContext(contextNode.DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault(), sModel, null, null, null, cToken);

            var hasAbatractClass = (new DecoratorAnalyzer.HasAbstractClass(abstractClass)).Analyze();
            var staticField = (new DecoratorAnalyzer.HasPrivateStaticField(contextNode)).Analyze();
            var decoratorAnalyzer = (new DecoratorAnalyzer.HasConstructor(contextNode)).Analyze();

            //Assert
            Assert.True((DecoratorAnalyzer._identifierValue == "Beverage"));
            Assert.True(hasAbatractClass && staticField && decoratorAnalyzer);
        }

        [Fact]
        public void Test_IsDecorator_Should_FAIL()
        {
            //Arrange
            var correctCode = @"
                    public class Milk: Beverage
                    {
                        private string baseBeverage;

                        public Milk(string beverage)
                        {
                            this.baseBeverage = beverage;
                        }

                        public override String Description
                        {
                            get
                            {
                                return baseBeverage.Description + "", Melk"";
                            }
                        }

                        public override double Cost()
                        {
                            return 0.35 + baseBeverage.Cost();
                        }
                    }

                    abstract class Beverage {
                        
                    }";

            var contextNode = SyntaxFactory.ParseSyntaxTree(correctCode).GetRoot();

            var workspace = new AdhocWorkspace()
                .AddSolution(SolutionInfo.Create(SolutionId.CreateNewId(), VersionStamp.Create(), null, null))
                .AddProject("DummyProject", "DummyProject", LanguageNames.CSharp)
                .AddMetadataReference(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
                .AddDocument("Decorator.cs", correctCode).Project.Solution;


            var dummyProject = workspace.Projects.FirstOrDefault();
            var cToken = new CancellationToken();
            var comp = dummyProject.GetCompilationAsync().Result;
            var sModel = comp.GetSemanticModel(comp.SyntaxTrees.First());
            var abstractClass = new SyntaxNodeAnalysisContext(contextNode.DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault(), sModel, null, null, null, cToken);

            // Act
            var hasAbatractClass = (new DecoratorAnalyzer.HasAbstractClass(abstractClass)).Analyze();
            var staticField = (new DecoratorAnalyzer.HasPrivateStaticField(contextNode)).Analyze();
            var decoratorAnalyzer = (new DecoratorAnalyzer.HasConstructor(contextNode)).Analyze();

            //Assert
            Assert.True((DecoratorAnalyzer._identifierValue == "Beverage"));
            Assert.False(hasAbatractClass && staticField && decoratorAnalyzer);
        }
    }
}
