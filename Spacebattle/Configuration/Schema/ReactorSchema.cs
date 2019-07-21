
namespace Spacebattle.Configuration.Schema
{
    public class ReactorSchema
    {
        public string Name { get;  set; }
        public float MaxHealth { get;  set; }
        public float Mass { get;  set; }
        public float UpkeepCost { get;  set; }
        public float MaxPower { get;  set; }

        public ReactorSchema() { }
        public ReactorSchema(string name, float maxHealth, float mass, float upkeepCost, float maxPower)
        {
            Name = name;
            MaxHealth = maxHealth;
            Mass = mass;
            UpkeepCost = upkeepCost;
            MaxPower = maxPower;
        }
    }
}
