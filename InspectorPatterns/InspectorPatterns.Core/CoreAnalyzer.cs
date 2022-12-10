using InspectorPatterns.Core.Models;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Text;

namespace InspectorPatterns.Core
{
    public class CoreAnalyzer
    {
        public Results Results { get; set; }
        public CoreAnalyzer(SyntaxNodeAnalysisContext context)
        {

        }
    }
}
