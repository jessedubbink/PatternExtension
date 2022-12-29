using DecoratorPattern.Beverages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmulatorProject.DesignPatterns.Decorator
{
    public abstract class Beverage
    {
        protected String baseDescription = "";
        public virtual String Description { get { return baseDescription; } }

        public abstract double Cost();
    }
}
