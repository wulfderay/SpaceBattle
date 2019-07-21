using Spacebattle.Configuration.Schema;
using Spacebattle.entity.parts.Weapon;

namespace Spacebattle.Configuration.StandardConfigs
{
    public class Weapons
    {

        public static WeaponSchema StandardPlasmaBolt()
        {

            //float maxHealth, float mass, float upkeepCost, float power, float range
            return new WeaponSchema()
            {
                Name = "Plasmabolt",
                WeaponType = WeaponType.PLASMABOLT,
                MaxHealth = 100,
                Mass = 10,
                UpkeepCost = 75,
                Power = 100,
                Range = 750

            };
        }

        public static WeaponSchema StandardTorpedoTube()
        {

            return new WeaponSchema()
            {
                Name = "Torp",
                WeaponType = WeaponType.TORPEDOTUBE,
                MaxHealth = 100,
                Mass = 100,
                UpkeepCost = 20,
                TorpedoType = "Doesn't Matter Yet"
            };
        }
    }
}
