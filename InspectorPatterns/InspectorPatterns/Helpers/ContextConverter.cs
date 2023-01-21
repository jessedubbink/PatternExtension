using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Text;

namespace InspectorPatterns.Helpers
{
    public static class ContextConverter
    {
        public static SyntaxNode Convert(SyntaxNodeAnalysisContext context)
        {
            return context.Node.SyntaxTree.GetRoot();
        }
    }
}
