using System;

namespace EmulatorProject.DesignPatterns.Decorator
{
    public class Espresso : Beverage
    {
        public Espresso() { baseDescription = "Espresso"; }
        public override double Cost() { return 1.99; }
    }
}
