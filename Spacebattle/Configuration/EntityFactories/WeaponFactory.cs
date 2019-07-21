using System;
using Spacebattle.Configuration.Schema;
using Spacebattle.entity.parts.Weapon;
using Spacebattle.Entity.parts.Weapon;

namespace Spacebattle.Configuration.EntityConstructors
{
    public static class WeaponFactory
    {
        public static IWeapon Construct(WeaponSchema weaponSchema)
        {
            switch (weaponSchema.WeaponType)
            {
                case WeaponType.PHASER:
                    return new Phaser(
                        weaponSchema.Name, 
                        weaponSchema.MaxHealth, 
                        weaponSchema.Mass, 
                        weaponSchema.UpkeepCost, 
                        weaponSchema.Power, 
                        weaponSchema.Range
                        );
                case WeaponType.FIREBREATH:
                    return new FireBreath(
                        weaponSchema.Name, 
                        weaponSchema.MaxHealth, 
                        weaponSchema.Mass, 
                        weaponSchema.UpkeepCost, 
                        weaponSchema.Power, 
                        weaponSchema.Range
                        );
                case WeaponType.LANCE:
                    return new Lance(
                        weaponSchema.Name, 
                        weaponSchema.MaxHealth, 
                        weaponSchema.Mass, 
                        weaponSchema.UpkeepCost, 
                        weaponSchema.Power, 
                        weaponSchema.Range
                        );
                case WeaponType.MASSDRIVER:
                    return new MassDriver(
                        weaponSchema.Name,
                         weaponSchema.MaxHealth,
                        weaponSchema.Mass,
                        weaponSchema.UpkeepCost,
                        weaponSchema.Power,
                        weaponSchema.Ammo,
                        weaponSchema.Range
                        );
                case WeaponType.PLASMABOLT:
                    return new PlasmaBolt(
                        weaponSchema.Name,
                        weaponSchema.MaxHealth,
                        weaponSchema.Mass,
                        weaponSchema.UpkeepCost,
                        weaponSchema.Power,
                        weaponSchema.Range
                        );
                case WeaponType.TORPEDOTUBE:
                    return new TorpedoTube(
                        weaponSchema.Name,
                        weaponSchema.MaxHealth,
                        weaponSchema.Mass,
                        weaponSchema.UpkeepCost,
                        GetTorpedoFunc(weaponSchema.TorpedoType)

                        );
                default:
                    throw new ArgumentOutOfRangeException(nameof(weaponSchema.WeaponType));
            }
        }

        private static Func<IDamageableEntity, Torpedo> GetTorpedoFunc(string torpedoType)
        {
            return defaultTorpedoFunc; // TODO torpedo types
        }

        private static Torpedo defaultTorpedoFunc(IDamageableEntity target)
        {
            
                return new Torpedo("torp", target, 15, 300, 120, 400); // TODo: proper torpedo naming.
            
        }
    }
}