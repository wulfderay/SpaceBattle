
using Spacebattle.Damage;
using System;

namespace Spacebattle.entity.parts.Weapon
{
    class Phaser:ShipPart,IWeapon
    {
        float _power;
        float _range;

        public Phaser(string name, float maxHealth, float mass, float upkeepCost, float power, float range): base(name, maxHealth, mass, upkeepCost)
        {
            _power = power;
            _range = range;
        }

        public void FireAt(IDamageableEntity target)
        {
            if (IsDestroyed())
                return;
            var distance = Parent.DistanceTo(target);
            if ( distance < _range)
                target.Damage(new DamageSource() { Magnitude = _power -(_power *distance/_range) , Type = DamageType.DRAINING, Origin = Parent.Position });
            else
                OnFlavourText(_name, "Target was too far away to hit!");
        }

        public WeaponType GetWeaponType()
        {
            return WeaponType.ENERGY;
        }

        public override string ToString()
        {
            return "[" + _name + " H:" + _currentHealth + "/" + _maxHealth + " P:" + _power + " R:" + _range + "]";
        }
    }
}
