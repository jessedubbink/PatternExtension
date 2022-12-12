using InspectorPatterns.Core.Interfaces;
using InspectorPatterns.Core.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Text;

namespace InspectorPatterns.Core
{
    public class DesignPatternAnalyzer
    {
        private IAnalyzer analyzer;
        public Location Location { get; set; }
        //public Results Results { get; set; }

        public DesignPatternAnalyzer() { }

        public DesignPatternAnalyzer(IAnalyzer analyzer)
        {
            this.analyzer = analyzer;
        }

        public void SetAnalyzerStrategy(IAnalyzer analyzer)
        {
            this.analyzer = analyzer;
        }

        public bool Analyze()
        {
            if (analyzer == null)
            {
                return false;
            }

            var result = analyzer.Analyze();

            if (result)
            {
                Location = analyzer.GetLocation();
            }

            return result;
        }
    }
}
