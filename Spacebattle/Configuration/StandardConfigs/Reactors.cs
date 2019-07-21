using Spacebattle.Configuration.Schema;

namespace Spacebattle.Configuration.StandardConfigs
{
    public static class Reactors
    {
        public static ReactorSchema BigReactor()
        {
            return new ReactorSchema("Big Reactor", 200, 200, 0, 500);
        }
        public static ReactorSchema BorgReactor()
        {
            return new ReactorSchema("Borg Reactor", 1000, 100, 0, 5000);
        }

        public static ReactorSchema SmallReactor()
        {
            return new ReactorSchema("Small Reactor", 100, 100, 0, 250);
        }
    }
}
