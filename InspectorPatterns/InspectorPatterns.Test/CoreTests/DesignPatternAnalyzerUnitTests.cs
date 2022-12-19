using InspectorPatterns.Core;
using Xunit;

namespace InspectorPatterns.Test.CoreTests
{
    public class DesignPatternAnalyzerUnitTests
    {
        [Fact]
        public void Test_ConvertContext_ShouldSucceed()
        {
            //Arrange
            var code = @"
                public sealed class Test
                {
                    public int Value { get; set; };
                }";
            var context = new CreateAnalysisContext(code).GetContext();

            //Act
            var designPatternAnalyzer = new DesignPatternAnalyzer(context);

            //Assert
            Assert.NotNull(designPatternAnalyzer.GetContext());
        }

        [Fact]
        public void Test_Analyze_ShouldSucceed()
        {
            //Arrange
            var code = "";
            var context = new CreateAnalysisContext(code).GetContext();
            var designPatternAnalyzer = new DesignPatternAnalyzer(context);
            designPatternAnalyzer.SetAnalyzerStrategy(new TestAnalyzer());

            //Act & Assert
            Assert.True(designPatternAnalyzer.Analyze());
        }
    }
}
