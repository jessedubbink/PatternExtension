using EmulatorProject.DesignPatterns;

namespace EmulatorProject
{
    public class EmulatorProject
    {
        public static void Main(string[] args)
        {
            #region "Singleton"

            Singleton s1 = Singleton.GetInstance();
            Singleton s2 = Singleton.GetInstance();

            if (s1 == s2)
            {
                Console.WriteLine("Singleton works, both variables contain the same instance.");
            }
            else
            {
                Console.WriteLine("Singleton failed, variables contain different instances.");
            }

            #endregion
        }
    }
}