using Spacebattle.Configuration.Schema;
using Spacebattle.entity.parts;

namespace Spacebattle.Configuration.EntityConstructors
{
    public static class ShieldFactory
    {
        public static Shield Construct(ShieldSchema shieldSchema)
        {
            return new Shield(shieldSchema.Name, shieldSchema.MaxHealth, shieldSchema.Mass, shieldSchema.UpkeepCost,
                shieldSchema.MaxAbsorbtion, shieldSchema.RegenRate, shieldSchema.PowerPerRegenUnit, shieldSchema.ArcWidthDegrees, shieldSchema.BaseAngle);
        }
    }
}