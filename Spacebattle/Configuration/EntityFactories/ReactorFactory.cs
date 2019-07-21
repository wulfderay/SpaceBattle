
using Spacebattle.Configuration.Schema;
using Spacebattle.entity.parts;

namespace Spacebattle.Configuration.EntityFactories
{
    public class ReactorFactory
    {
        public static Reactor Construct(ReactorSchema reactorSchema)
        {
            return new Reactor(reactorSchema.Name, reactorSchema.MaxHealth, reactorSchema.Mass, reactorSchema.UpkeepCost, reactorSchema.MaxPower);
        }
    }
}
