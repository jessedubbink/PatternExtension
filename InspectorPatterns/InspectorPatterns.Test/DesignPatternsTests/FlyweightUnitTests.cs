using InspectorPatterns.Core.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Linq;
using System.Threading;
using Xunit;
using static InspectorPatterns.Core.Analyzers.FactoryMethodAnalyzer;

namespace InspectorPatterns.Test.DesignPatternsTests
{
    public class FlyweightUnitTests
    {
        [Fact]
        public void Test_HasFlyweightCollection_NoCollection_ShouldReturnFalse()
        {
            //Arrange
            var flyweightClass = @"
                public class FlyweightFactory
                {
                    private Flyweight flyweights = new Flyweight();
                }"
            ;

            var fieldDeclaration = SyntaxFactory.ParseSyntaxTree(flyweightClass).GetRoot().DescendantNodes().OfType<FieldDeclarationSyntax>().FirstOrDefault();

            var workspace = new AdhocWorkspace()
                .AddSolution(SolutionInfo.Create(SolutionId.CreateNewId(), VersionStamp.Create(), null, null))
                .AddProject("DummyProject", "DummyProject", LanguageNames.CSharp)
                .AddMetadataReference(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
                .AddDocument("FlyweightFactory.cs", flyweightClass).Project.Solution;

            var dummyProject = workspace.Projects.FirstOrDefault();
            var cToken = new CancellationToken();
            var comp = dummyProject.GetCompilationAsync().Result;
            var sModel = comp.GetSemanticModel(comp.SyntaxTrees.First());
            var context = new SyntaxNodeAnalysisContext(fieldDeclaration, sModel, null, null, null, cToken);
            var flyweightAnalyzer = new FlyweightAnalyzer.HasFlyweightCollection(context);

            //Act
            var result = flyweightAnalyzer.Analyze();

            //Assert
            Assert.False(result);
        }

        [Fact]
        public void Test_HasFlyweightCollection_IsListCollection_ShouldReturnTrue()
        {
            //Arrange
            var flyweightClass = @"
                public class FlyweightFactory
                {
                    private List<Flyweight> flyweights = new List<Flyweight>();
                }"
            ;

            var fieldDeclaration = SyntaxFactory.ParseSyntaxTree(flyweightClass).GetRoot().DescendantNodes().OfType<FieldDeclarationSyntax>().FirstOrDefault();

            var workspace = new AdhocWorkspace()
                .AddSolution(SolutionInfo.Create(SolutionId.CreateNewId(), VersionStamp.Create(), null, null))
                .AddProject("DummyProject", "DummyProject", LanguageNames.CSharp)
                .AddMetadataReference(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
                .AddDocument("FlyweightFactory.cs", flyweightClass).Project.Solution;

            var dummyProject = workspace.Projects.FirstOrDefault();
            var cToken = new CancellationToken();
            var comp = dummyProject.GetCompilationAsync().Result;
            var sModel = comp.GetSemanticModel(comp.SyntaxTrees.First());
            var context = new SyntaxNodeAnalysisContext(fieldDeclaration, sModel, null, null, null, cToken);
            var flyweightAnalyzer = new FlyweightAnalyzer.HasFlyweightCollection(context);

            //Act
            var result = flyweightAnalyzer.Analyze();

            //Assert
            Assert.True(result);
        }

        [Fact]
        public void Test_HasFlyweightCollection_IsDictionaryCollection_ShouldReturnTrue()
        {
            //Arrange
            var flyweightClass = @"
                public class FlyweightFactory
                {
                    private Dictionary<Flyweight> flyweights = new Dictionary<Flyweight>();
                }"
            ;

            var fieldDeclaration = SyntaxFactory.ParseSyntaxTree(flyweightClass).GetRoot().DescendantNodes().OfType<FieldDeclarationSyntax>().FirstOrDefault();

            var workspace = new AdhocWorkspace()
                .AddSolution(SolutionInfo.Create(SolutionId.CreateNewId(), VersionStamp.Create(), null, null))
                .AddProject("DummyProject", "DummyProject", LanguageNames.CSharp)
                .AddMetadataReference(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
                .AddDocument("FlyweightFactory.cs", flyweightClass).Project.Solution;

            var dummyProject = workspace.Projects.FirstOrDefault();
            var cToken = new CancellationToken();
            var comp = dummyProject.GetCompilationAsync().Result;
            var sModel = comp.GetSemanticModel(comp.SyntaxTrees.First());
            var context = new SyntaxNodeAnalysisContext(fieldDeclaration, sModel, null, null, null, cToken);
            var flyweightAnalyzer = new FlyweightAnalyzer.HasFlyweightCollection(context);

            //Act
            var result = flyweightAnalyzer.Analyze();

            //Assert
            Assert.True(result);
        }

        [Fact]
        public void Test_HasGetFlyweightMethod_IsMethodPublic_ShouldReturnFalse()
        {
            //Arrange
            var flyweightClass = @"
                public class FlyweightFactory
                {
                    private Dictionary<Flyweight> flyweights = new Dictionary<Flyweight>();

                    private Flyweight GetFlyweight(Car sharedState)
                    {
                        string key = getKey(sharedState);

                        if (flyweights.Where(t => t.getFlyweightKey() == key).Count() == 0)
                        {
                            Console.WriteLine(""FlyweightFactory: Can't find a flyweight, creating new one."");
                            flyweights.Add(new Flyweight(sharedState));
                        }
                        else
                        {
                            Console.WriteLine(""FlyweightFactory: Reusing existing flyweight."");
                        }
                        return flyweights.Where(t => t.getFlyweightKey() == key).FirstOrDefault();
                    }
                }"
            ;

            var methodDeclaration = SyntaxFactory.ParseSyntaxTree(flyweightClass).GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>().FirstOrDefault();

            var workspace = new AdhocWorkspace()
                .AddSolution(SolutionInfo.Create(SolutionId.CreateNewId(), VersionStamp.Create(), null, null))
                .AddProject("DummyProject", "DummyProject", LanguageNames.CSharp)
                .AddMetadataReference(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
                .AddDocument("FlyweightFactory.cs", flyweightClass).Project.Solution;

            var dummyProject = workspace.Projects.FirstOrDefault();
            var cToken = new CancellationToken();
            var comp = dummyProject.GetCompilationAsync().Result;
            var sModel = comp.GetSemanticModel(comp.SyntaxTrees.First());
            var context = new SyntaxNodeAnalysisContext(methodDeclaration, sModel, null, null, null, cToken);
            var flyweightAnalyzer = new FlyweightAnalyzer.HasGetFlyweightMethod(context);

            //Act
            var result = flyweightAnalyzer.Analyze();

            //Assert
            Assert.False(result);
        }

        [Fact]
        public void Test_HasGetFlyweightMethod_IsMethodPublic_ShouldReturnTrue()
        {
            //Arrange
            var flyweightClass = @"
                public class FlyweightFactory
                {
                    private Dictionary<Flyweight> flyweights = new Dictionary<Flyweight>();

                    public Flyweight GetFlyweight(Car sharedState)
                    {
                        string key = getKey(sharedState);

                        if (flyweights.Where(t => t.getFlyweightKey() == key).Count() == 0)
                        {
                            Console.WriteLine(""FlyweightFactory: Can't find a flyweight, creating new one."");
                            flyweights.Add(new Flyweight(sharedState));
                        }
                        else
                        {
                            Console.WriteLine(""FlyweightFactory: Reusing existing flyweight."");
                        }
                        return flyweights.Where(t => t.getFlyweightKey() == key).FirstOrDefault();
                    }
                }"
            ;

            var methodDeclaration = SyntaxFactory.ParseSyntaxTree(flyweightClass).GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>().FirstOrDefault();

            var workspace = new AdhocWorkspace()
                .AddSolution(SolutionInfo.Create(SolutionId.CreateNewId(), VersionStamp.Create(), null, null))
                .AddProject("DummyProject", "DummyProject", LanguageNames.CSharp)
                .AddMetadataReference(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
                .AddDocument("FlyweightFactory.cs", flyweightClass).Project.Solution;

            var dummyProject = workspace.Projects.FirstOrDefault();
            var cToken = new CancellationToken();
            var comp = dummyProject.GetCompilationAsync().Result;
            var sModel = comp.GetSemanticModel(comp.SyntaxTrees.First());
            var context = new SyntaxNodeAnalysisContext(methodDeclaration, sModel, null, null, null, cToken);
            var flyweightAnalyzer = new FlyweightAnalyzer.HasGetFlyweightMethod(context);

            //Act
            var result = flyweightAnalyzer.Analyze();

            //Assert
            Assert.True(result);
        }

        [Fact]
        public void Test_HasGetFlyweightMethod_IsReturnTypeNotNull_ShouldReturnFalse()
        {
            //Arrange
            var flyweightClass = @"
                public class FlyweightFactory
                {
                    private Dictionary<Flyweight> flyweights = new Dictionary<Flyweight>();

                    private void GetFlyweight(Car sharedState)
                    {
                        string key = getKey(sharedState);

                        if (flyweights.Where(t => t.getFlyweightKey() == key).Count() == 0)
                        {
                            Console.WriteLine(""FlyweightFactory: Can't find a flyweight, creating new one."");
                            flyweights.Add(new Flyweight(sharedState));
                        }
                        else
                        {
                            Console.WriteLine(""FlyweightFactory: Reusing existing flyweight."");
                        }
                    }
                }"
            ;

            var methodDeclaration = SyntaxFactory.ParseSyntaxTree(flyweightClass).GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>().FirstOrDefault();

            var workspace = new AdhocWorkspace()
                .AddSolution(SolutionInfo.Create(SolutionId.CreateNewId(), VersionStamp.Create(), null, null))
                .AddProject("DummyProject", "DummyProject", LanguageNames.CSharp)
                .AddMetadataReference(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
                .AddDocument("FlyweightFactory.cs", flyweightClass).Project.Solution;

            var dummyProject = workspace.Projects.FirstOrDefault();
            var cToken = new CancellationToken();
            var comp = dummyProject.GetCompilationAsync().Result;
            var sModel = comp.GetSemanticModel(comp.SyntaxTrees.First());
            var context = new SyntaxNodeAnalysisContext(methodDeclaration, sModel, null, null, null, cToken);
            var flyweightAnalyzer = new FlyweightAnalyzer.HasGetFlyweightMethod(context);

            //Act
            var result = flyweightAnalyzer.Analyze();

            //Assert
            Assert.False(result);
        }

        [Fact]
        public void Test_HasGetFlyweightMethod_IsReturnTypeNotNull_ShouldReturnTrue()
        {
            //Arrange
            var flyweightClass = @"
                public class FlyweightFactory
                {
                    private Dictionary<Flyweight> flyweights = new Dictionary<Flyweight>();

                    public void GetFlyweight(Car sharedState)
                    {
                        string key = getKey(sharedState);

                        if (flyweights.Where(t => t.getFlyweightKey() == key).Count() == 0)
                        {
                            Console.WriteLine(""FlyweightFactory: Can't find a flyweight, creating new one."");
                            flyweights.Add(new Flyweight(sharedState));
                        }
                        else
                        {
                            Console.WriteLine(""FlyweightFactory: Reusing existing flyweight."");
                        }
                        return flyweights.Where(t => t.getFlyweightKey() == key).FirstOrDefault();
                    }
                }"
            ;

            var methodDeclaration = SyntaxFactory.ParseSyntaxTree(flyweightClass).GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>().FirstOrDefault();

            var workspace = new AdhocWorkspace()
                .AddSolution(SolutionInfo.Create(SolutionId.CreateNewId(), VersionStamp.Create(), null, null))
                .AddProject("DummyProject", "DummyProject", LanguageNames.CSharp)
                .AddMetadataReference(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
                .AddDocument("FlyweightFactory.cs", flyweightClass).Project.Solution;

            var dummyProject = workspace.Projects.FirstOrDefault();
            var cToken = new CancellationToken();
            var comp = dummyProject.GetCompilationAsync().Result;
            var sModel = comp.GetSemanticModel(comp.SyntaxTrees.First());
            var context = new SyntaxNodeAnalysisContext(methodDeclaration, sModel, null, null, null, cToken);
            var flyweightAnalyzer = new FlyweightAnalyzer.HasGetFlyweightMethod(context);

            //Act
            var result = flyweightAnalyzer.Analyze();

            //Assert
            Assert.True(result);
        }

        [Fact]
        public void Test_HasGetFlyweightMethod_IsMethodBodyNotEmpty_ShouldReturnTrue()
        {
            //Arrange
            var flyweightClass = @"
                public class FlyweightFactory
                {
                    private Dictionary<Flyweight> flyweights = new Dictionary<Flyweight>();

                    public void GetFlyweight(Car sharedState)
                    {
                        string key = getKey(sharedState);

                        if (flyweights.Where(t => t.getFlyweightKey() == key).Count() == 0)
                        {
                            Console.WriteLine(""FlyweightFactory: Can't find a flyweight, creating new one."");
                            flyweights.Add(new Flyweight(sharedState));
                        }
                        else
                        {
                            Console.WriteLine(""FlyweightFactory: Reusing existing flyweight."");
                        }
                        return flyweights.Where(t => t.getFlyweightKey() == key).FirstOrDefault();
                    }
                }"
            ;

            var methodDeclaration = SyntaxFactory.ParseSyntaxTree(flyweightClass).GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>().FirstOrDefault();

            var workspace = new AdhocWorkspace()
                .AddSolution(SolutionInfo.Create(SolutionId.CreateNewId(), VersionStamp.Create(), null, null))
                .AddProject("DummyProject", "DummyProject", LanguageNames.CSharp)
                .AddMetadataReference(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
                .AddDocument("FlyweightFactory.cs", flyweightClass).Project.Solution;

            var dummyProject = workspace.Projects.FirstOrDefault();
            var cToken = new CancellationToken();
            var comp = dummyProject.GetCompilationAsync().Result;
            var sModel = comp.GetSemanticModel(comp.SyntaxTrees.First());
            var context = new SyntaxNodeAnalysisContext(methodDeclaration, sModel, null, null, null, cToken);
            var flyweightAnalyzer = new FlyweightAnalyzer.HasGetFlyweightMethod(context);

            //Act
            var result = flyweightAnalyzer.Analyze();

            //Assert
            Assert.True(result);
        }

        [Fact]
        public void Test_HasGetFlyweightMethod_IsMethodBodyNotEmpty_ShouldReturnFalse()
        {
            //Arrange
            var flyweightClass = @"
                public class FlyweightFactory
                {
                    private Dictionary<Flyweight> flyweights = new Dictionary<Flyweight>();

                    public void GetFlyweight(Car sharedState)
                    { }
                }"
            ;

            var methodDeclaration = SyntaxFactory.ParseSyntaxTree(flyweightClass).GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>().FirstOrDefault();

            var workspace = new AdhocWorkspace()
                .AddSolution(SolutionInfo.Create(SolutionId.CreateNewId(), VersionStamp.Create(), null, null))
                .AddProject("DummyProject", "DummyProject", LanguageNames.CSharp)
                .AddMetadataReference(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
                .AddDocument("FlyweightFactory.cs", flyweightClass).Project.Solution;

            var dummyProject = workspace.Projects.FirstOrDefault();
            var cToken = new CancellationToken();
            var comp = dummyProject.GetCompilationAsync().Result;
            var sModel = comp.GetSemanticModel(comp.SyntaxTrees.First());
            var context = new SyntaxNodeAnalysisContext(methodDeclaration, sModel, null, null, null, cToken);
            var flyweightAnalyzer = new FlyweightAnalyzer.HasGetFlyweightMethod(context);

            //Act
            var result = flyweightAnalyzer.Analyze();

            //Assert
            Assert.False(result);
        }

        [Fact]
        public void Test_HasGetFlyweightMethod_IsObjectCreated_ShouldReturnTrue()
        {
            //Arrange
            var flyweightClass = @"
                public class FlyweightFactory
                {
                    private Dictionary<Flyweight> flyweights = new Dictionary<Flyweight>();

                    public void GetFlyweight(Car sharedState)
                    {
                        string key = getKey(sharedState);

                        if (flyweights.Where(t => t.getFlyweightKey() == key).Count() == 0)
                        {
                            Console.WriteLine(""FlyweightFactory: Can't find a flyweight, creating new one."");
                            flyweights.Add(new Flyweight(sharedState));
                        }
                        else
                        {
                            Console.WriteLine(""FlyweightFactory: Reusing existing flyweight."");
                        }
                        return flyweights.Where(t => t.getFlyweightKey() == key).FirstOrDefault();
                    }
                }"
            ;

            var methodDeclaration = SyntaxFactory.ParseSyntaxTree(flyweightClass).GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>().FirstOrDefault();

            var workspace = new AdhocWorkspace()
                .AddSolution(SolutionInfo.Create(SolutionId.CreateNewId(), VersionStamp.Create(), null, null))
                .AddProject("DummyProject", "DummyProject", LanguageNames.CSharp)
                .AddMetadataReference(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
                .AddDocument("FlyweightFactory.cs", flyweightClass).Project.Solution;

            var dummyProject = workspace.Projects.FirstOrDefault();
            var cToken = new CancellationToken();
            var comp = dummyProject.GetCompilationAsync().Result;
            var sModel = comp.GetSemanticModel(comp.SyntaxTrees.First());
            var context = new SyntaxNodeAnalysisContext(methodDeclaration, sModel, null, null, null, cToken);
            var flyweightAnalyzer = new FlyweightAnalyzer.HasGetFlyweightMethod(context);

            //Act
            var result = flyweightAnalyzer.Analyze();

            //Assert
            Assert.True(result);
        }

        [Fact]
        public void Test_HasGetFlyweightMethod_IsObjectCreated_ShouldReturnFalse()
        {
            //Arrange
            var flyweightClass = @"
                public class FlyweightFactory
                {
                    private Dictionary<Flyweight> flyweights = new Dictionary<Flyweight>();

                    public void GetFlyweight(Car sharedState)
                    {
                        string key = getKey(sharedState);

                        if (flyweights.Where(t => t.getFlyweightKey() == key).Count() == 0)
                        {
                            Console.WriteLine(""FlyweightFactory: Can't find a flyweight, creating new one."");
                            flyweights.Add(new Car(sharedState));
                        }
                        else
                        {
                            Console.WriteLine(""FlyweightFactory: Reusing existing flyweight."");
                        }
                        return flyweights.Where(t => t.getFlyweightKey() == key).FirstOrDefault();
                    }
                }"
            ;

            var methodDeclaration = SyntaxFactory.ParseSyntaxTree(flyweightClass).GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>().FirstOrDefault();

            var workspace = new AdhocWorkspace()
                .AddSolution(SolutionInfo.Create(SolutionId.CreateNewId(), VersionStamp.Create(), null, null))
                .AddProject("DummyProject", "DummyProject", LanguageNames.CSharp)
                .AddMetadataReference(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
                .AddDocument("FlyweightFactory.cs", flyweightClass).Project.Solution;

            var dummyProject = workspace.Projects.FirstOrDefault();
            var cToken = new CancellationToken();
            var comp = dummyProject.GetCompilationAsync().Result;
            var sModel = comp.GetSemanticModel(comp.SyntaxTrees.First());
            var context = new SyntaxNodeAnalysisContext(methodDeclaration, sModel, null, null, null, cToken);
            var flyweightAnalyzer = new FlyweightAnalyzer.HasGetFlyweightMethod(context);

            //Act
            var result = flyweightAnalyzer.Analyze();

            //Assert
            Assert.False(result);
        }
    }
}
