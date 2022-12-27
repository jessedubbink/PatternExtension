using InspectorPatterns.Core.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System;

namespace InspectorPatterns.Core
{
    public class DesignPatternAnalyzer
    {
        private IAnalyzer _analyzer;

        public void SetAnalyzerStrategy(IAnalyzer analyzer)
        {
            _analyzer = analyzer;
        }

        public bool Analyze()
        {
            if (_analyzer == null)
            {
                return false;
            }

            var result = _analyzer.Analyze();

            return result;
        }
    }
}
