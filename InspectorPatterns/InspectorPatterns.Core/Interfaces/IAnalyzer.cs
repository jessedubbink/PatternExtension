using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace InspectorPatterns.Core.Interfaces
{
    public interface IAnalyzer
    {
        bool Analyze();
        Location GetLocation();
    }
}
