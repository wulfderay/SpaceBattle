namespace Spacebattle.Configuration.Schema
{
    public class CrewDeckSchema
    {
        public string Name { get; set; }
        public float MaxHealth { get; set; }
        public float Mass { get; set; }
        public float UpkeepCostPerCrew { get; set; }
        public uint CrewCompliment { get; set; }
        public float RepairRate { get; set; }

        public CrewDeckSchema() { }
        public CrewDeckSchema(string name, float maxHealth, float mass, float upkeepCostPerCrew, uint crewCompliment, float repairRate)
        {
            Name = name;
            Mass = mass;
            MaxHealth = maxHealth;
            UpkeepCostPerCrew = upkeepCostPerCrew;
            CrewCompliment = crewCompliment;
            RepairRate = repairRate;
        }
    }
}
