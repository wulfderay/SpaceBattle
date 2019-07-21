using Spacebattle.Configuration.Schema;
using Spacebattle.entity.parts;

namespace Spacebattle.Configuration.EntityConstructors
{
    public static class EngineFactory
    {
        public static Engine Construct(EngineSchema engineSchema)
        {
            return new Engine(engineSchema.Name, engineSchema.MaxHealth, engineSchema.Mass, engineSchema.UpkeepCost, engineSchema.ThrustPower);
        }
    }
}