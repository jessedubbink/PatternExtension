using System;

namespace EmulatorProject.DesignPatterns.Decorator
{
	public class Test
	{
		public Test()
		{
			Beverage beverage = new Milk(new Soy(new Espresso()));
		}
	}
}
