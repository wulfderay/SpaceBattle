

using System;
using Spacebattle.Damage;
namespace Spacebattle.entity.parts
{
    //Todo: model reloading
    public class Weapon : ShipPart
    {

        float _power;
        float _range;

        public Weapon(string name, float maxHealth, float mass, float upkeepCost,float power, float range): base(name, maxHealth, mass, upkeepCost)
        {
            _power = power;
            _range = range;
        }

        /// <summary>
        /// Damages a target if it's close enough. Note that further implemtations could make damage fall off with the distance or make it more acurate at short distances etc.
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="target"></param>
        public void Shoot(float distance, IDamageable target)
        {
            if (IsDestroyed())
                return;
            if (distance <= _range) 
                target.Damage(new DamageSource() { Magnitude = _power, Type=DamageType.CONCUSSIVE });
            else
                OnFlavourText(_name, "Target was too far away to hit!");
        }

        public override string ToString()
        {
            return "["+_name+" H:" + _currentHealth + "/" + _maxHealth + " P:" + _power + " R:"+_range+"]";
        }
    }
}
