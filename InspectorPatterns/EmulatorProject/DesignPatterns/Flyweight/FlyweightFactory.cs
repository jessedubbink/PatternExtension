namespace EmulatorProject.DesignPatterns.Flyweight
{
    // The Flyweight Factory creates and manages the Flyweight objects. It
    // ensures that flyweights are shared correctly. When the client requests a
    // flyweight, the factory either returns an existing instance or creates a
    // new one, if it doesn't exist yet.
    public class FlyweightFactory
    {
        private List<Flyweight> flyweights = new List<Flyweight>();

        public FlyweightFactory(params Car[] args)
        {
            foreach (var elem in args)
            {
                flyweights.Add(new Flyweight(elem));
            }
        }

        // Returns a Flyweight's string hash for a given state.
        public string getKey(Car key)
        {
            List<string> elements = new List<string>();

            elements.Add(key.Model);
            elements.Add(key.Color);
            elements.Add(key.Company);

            if (key.Owner != null && key.Number != null)
            {
                elements.Add(key.Number);
                elements.Add(key.Owner);
            }

            elements.Sort();

            return string.Join("_", elements);
        }

        // Returns an existing Flyweight with a given state or creates a new
        // one.
        public Flyweight GetFlyweight(Car sharedState)
        {
            string key = getKey(sharedState);

            if (flyweights.Where(t => t.getFlyweightKey() == key).Count() == 0)
            {
                Console.WriteLine("FlyweightFactory: Can't find a flyweight, creating new one.");
                flyweights.Add(new Flyweight(sharedState));
            }
            else
            {
                Console.WriteLine("FlyweightFactory: Reusing existing flyweight.");
            }
            return flyweights.Where(t => t.getFlyweightKey() == key).FirstOrDefault();
        }
    }
}
