using System;
using Spacebattle.entity;
using Spacebattle.entity.parts.Weapon;
using Spacebattle.Damage;

namespace Spacebattle.Entity.parts.Weapon
{
    class PlasmaBolt : ShipPart, IWeapon
    {
        float _power;
        float _range;
        /// <summary>
        /// A crackling shot of plasma that destabilizes sheilds and melts hulls to slag
        /// Deals half of its damage as draining, half as fire.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="maxHealth"></param>
        /// <param name="mass"></param>
        /// <param name="upkeepCost"></param>
        /// <param name="power"></param>
        /// <param name="range"></param>
        public PlasmaBolt(string name, float maxHealth, float mass, float upkeepCost, float power, float range) : base(name, maxHealth, mass, upkeepCost)
        {
            _power = power;
            _range = range;
        }

        public void FireAt(IDamageableEntity target)
        {
            if (IsDestroyed())
                return;
            var distance = Parent.DistanceTo(target);
            if (distance < _range)
            {
                target.Damage(new DamageSource() { Magnitude = (_power  - (_power * distance / _range))/2, Type = DamageType.DRAINING, Origin = Parent.Position });
                target.Damage(new DamageSource() { Magnitude = _power / 2, Type = DamageType.FIRE, Origin = Parent.Position });
            }
            else
                OnFlavourText(_name, "Target was too far away to hit!");
        }

        public WeaponType GetWeaponType()
        {
            return WeaponType.ENERGY;
        }
    }
}
