using InspectorPatterns.Core;
using Xunit;

namespace InspectorPatterns.Test.CoreTests
{
    public class DesignPatternAnalyzerUnitTests
    {
        [Fact]
        public void Test_Analyze_ShouldSucceed()
        {
            //Arrange
            var designPatternAnalyzer = new DesignPatternAnalyzer();
            designPatternAnalyzer.SetAnalyzerStrategy(new TestAnalyzer());

            //Act & Assert
            Assert.True(designPatternAnalyzer.Analyze());
        }
    }
}
