using System;
using System.Collections.Generic;
using System.Text;

namespace InspectorPatterns.Core.DesignPatterns.Interfaces
{
    internal interface ISingletonPattern
    {
        bool HasPrivateConstructor();
        bool HasPrivateStaticSelf();
        bool HasGetInstance();
    }
}
