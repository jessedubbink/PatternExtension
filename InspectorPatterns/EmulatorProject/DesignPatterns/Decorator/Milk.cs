using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmulatorProject.DesignPatterns.Decorator;

namespace EmulatorProject.DesignPatterns.Decorator
{
    public class Milk: Beverage
    {
        private Beverage baseBeverage;

        public Milk(Beverage beverage)
        {
            this.baseBeverage = beverage;
        }

        public override String Description
        {
            get
            {
                return baseBeverage.Description + ", Melk";
            }
        }

        public override double Cost()
        {
            return 0.35 + baseBeverage.Cost();
        }
    }
}
