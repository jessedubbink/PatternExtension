using InspectorPatterns.Core.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace InspectorPatterns.Test
{
    public class TestAnalyzer : IAnalyzer
    {
        public bool Analyze()
        {
            return true;
        }

        public Location GetLocation()
        {
            return Location.Create("", new TextSpan(), new LinePositionSpan());
        }
    }
}
