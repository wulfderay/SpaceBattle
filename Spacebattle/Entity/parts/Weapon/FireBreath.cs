
using System;
using Spacebattle.Damage;

namespace Spacebattle.entity.parts.Weapon
{
    class FireBreath:ShipPart,IWeapon
    {
        float _power;
        float _range;
        /// <summary>
        /// This was mostly just for Saoirse. Not sure that it will make it into the game.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="maxHealth"></param>
        /// <param name="mass"></param>
        /// <param name="upkeepCost"></param>
        /// <param name="power"></param>
        /// <param name="range"></param>
        public FireBreath(string name, float maxHealth, float mass, float upkeepCost, float power, float range): base(name, maxHealth, mass, upkeepCost)
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
                target.Damage(new DamageSource() { Magnitude = _power -(_power *distance/_range) , Type = DamageType.FIRE, Origin = Parent.Position });
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
