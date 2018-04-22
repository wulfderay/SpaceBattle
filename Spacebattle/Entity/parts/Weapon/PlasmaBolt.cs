using System;
using Spacebattle.entity;
using Spacebattle.entity.parts.Weapon;
using Spacebattle.Damage;
using Spacebattle.Game;

namespace Spacebattle.Entity.parts.Weapon
{
    class PlasmaBolt : ShipPart, IWeapon
    {
        float _power;
        float _range;
        private IDamageableEntity _target;

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

        public void Fire()
        {
            if (IsDestroyed())
                return;
            if (_target == null)
            {
                OnFlavourText(_name, "No target to fire at!");
                return;
            }
            var distance = Parent.DistanceTo(_target);
            if (distance < _range)
            {
                OnGameEngineEvent(this, GameEngineEventArgs.Damage(_target, new DamageSource() { Magnitude = (_power - (_power * distance / _range)) / 2, Type = DamageType.DRAINING, Origin = Parent.Position }));
                OnGameEngineEvent(this, GameEngineEventArgs.Damage(_target, new DamageSource() { Magnitude = _power / 2, Type = DamageType.FIRE, Origin = Parent.Position }));
            }
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
    }
}
