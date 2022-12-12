using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmulatorProject.DesignPatterns.FactoryMethod
{
    public class ConcreteCreator1 : Creator
    {
        public override IProduct FactoryMethod()
        {
            return new ConcreteProduct1();
        }
    }
}
