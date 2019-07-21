using System.Collections.Generic;

namespace Spacebattle.Configuration.Schema
{
    public class ShipSchema
    {
        public string Name { get; set; }
        public List<CrewDeckSchema> CrewDecks { get; set; }
        public List<EngineSchema> Engines { get; set; }
        public List<ReactorSchema> Reactors { get; set; }
        public List<ShieldSchema> Shields { get; set; }
        public List<WeaponSchema> Weapons { get; set; }
    }
}
