
using System;
using Spacebattle.Damage;
using Spacebattle.Game;

namespace Spacebattle.entity.parts.Weapon
{
    class FireBreath:ShipPart,IWeapon
    {
        float _power;
        float _range;
        private IDamageableEntity _target;

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

        public void Fire()
        {
            if (IsDestroyed())
                return;
            if (_target == null)
            {
                OnFlavourText(_name, "No target to fire at!");
                return;
            }
            var distance = Parent.Position.DistanceTo(_target.Position);
            if (distance < _range)
                OnGameEngineEvent(this, GameEngineEventArgs.Damage(_target, new DamageSource() { Magnitude = _power - (_power * distance / _range), Type = DamageType.FIRE, Origin = Parent.Position }));
            else
                OnFlavourText(_name, "Target was too far away to hit!");
        }

        
        public IDamageableEntity GetLockTarget()
        {
            return _target;
        }

        public WeaponType GetWeaponType()
        {
            return WeaponType.ENERGY;
        }

        public void Lock(IDamageableEntity target)
        {
            _target = target;
        }

        public override string ToString()
        {
            return "[" + _name + " H:" + _currentHealth + "/" + _maxHealth + " P:" + _power + " R:" + _range + "]";
        }
    }
}
