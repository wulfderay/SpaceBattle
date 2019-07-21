using Spacebattle.Configuration.Schema;

namespace Spacebattle.Configuration.StandardConfigs
{
    public class Engines
    {
        public static EngineSchema Thruster()
        {
            return new EngineSchema("Thruster", 100, 15, 10, 100);
        }
        public static EngineSchema CoreDrive()
        {
            return new EngineSchema("Core Drive", 100, 70, 25, 250);
        }
        public static EngineSchema MainSail()
        {
            return new EngineSchema("MainSail", 100, 150, 40, 400);
        }
        public static EngineSchema MegaThruster()
        {
            return new EngineSchema("MegaThruster", 150, 450, 100, 1000);
        }
    }
}
