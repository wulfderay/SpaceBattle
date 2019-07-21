using Spacebattle.Configuration.Schema;
using Spacebattle.entity.parts;

namespace Spacebattle.Configuration.EntityConstructors
{
    public class CrewDeckFactory
    {
        public static CrewDeck Construct(CrewDeckSchema crewDeckSchema)
        {
            return new CrewDeck(crewDeckSchema.Name, crewDeckSchema.MaxHealth, crewDeckSchema.Mass, crewDeckSchema.UpkeepCostPerCrew, crewDeckSchema.CrewCompliment, crewDeckSchema.RepairRate);
        }
    }
}