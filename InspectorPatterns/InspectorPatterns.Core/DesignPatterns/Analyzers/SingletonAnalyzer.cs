using InspectorPatterns.Core.DesignPatterns.Interfaces;
using InspectorPatterns.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace InspectorPatterns.Core.DesignPatterns.Analyzers
{
    public class SingletonAnalyzer : IAnalyzer, ISingletonPattern
    {
        public void Analyze()
        {
            throw new NotImplementedException();
        }

        public bool HasGetInstance()
        {
            throw new NotImplementedException();
        }

        public bool HasPrivateConstructor()
        {
            throw new NotImplementedException();
        }

        public bool HasStaticSelf()
        {
            throw new NotImplementedException();
        }
    }
}
