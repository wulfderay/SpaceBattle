namespace Spacebattle.Configuration.Schema
{
    public class EngineSchema
    {
        public string Name { get;  set; }
        public float MaxHealth { get;  set; }
        public float Mass { get;  set; }
        public float UpkeepCost { get;  set; }
        public float ThrustPower { get;  set; }

        public EngineSchema() { }
        public EngineSchema(string name, float maxHealth, float mass, float upkeepCost, float thrustPower)
        {
            Name = name;
            MaxHealth = maxHealth;
            Mass = mass;
            UpkeepCost = upkeepCost;
            ThrustPower = thrustPower;
        }
    }
}
