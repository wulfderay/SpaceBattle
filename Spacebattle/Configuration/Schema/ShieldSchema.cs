
namespace Spacebattle.Configuration.Schema
{
    public class ShieldSchema
    {


        public string Name { get;  set; }
        public float MaxHealth { get;  set; }
        public float Mass { get;  set; }
        public float UpkeepCost { get;  set; }
        public float MaxAbsorbtion { get;  set; }
        public float RegenRate { get; set; }
        public float PowerPerRegenUnit { get;  set; }
        public float ArcWidthDegrees { get;  set; }
        public float BaseAngle { get;  set; }

        public ShieldSchema() { }
        public ShieldSchema(string name, float maxHealth, float mass, float upkeepCost, float maxAbsorbtion, float regenRate, float powerPerRegenUnit, float arcWidthDegrees, float baseAngle)
        {
            Name = name;
            MaxHealth = maxHealth;
            Mass = mass;
            UpkeepCost = upkeepCost;
            MaxAbsorbtion = maxAbsorbtion;
            RegenRate = regenRate;
            PowerPerRegenUnit = powerPerRegenUnit;
            ArcWidthDegrees = arcWidthDegrees;
            BaseAngle = baseAngle;
        }
    }
}
