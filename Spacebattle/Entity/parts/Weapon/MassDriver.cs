using Spacebattle.entity.parts.Weapon;
using Spacebattle.entity;
using Spacebattle.Damage;
using System;
using Spacebattle.Game;

namespace Spacebattle.Entity.parts.Weapon
{
    class MassDriver : ShipPart, IWeapon
    {
        uint _ammo;
        float _power;
        float _range;
        private IDamageableEntity _target;

        public MassDriver(string name, float maxHealth, float mass, float upkeepCost, float power, uint ammo, float range) : base(name, maxHealth, mass, upkeepCost)
        {
            _ammo = ammo;
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
            if (_ammo <= 0)
            {
                OnFlavourText(_name, "No more ammo!");
                return;
            }
            var distance = Parent.DistanceTo(_target);
            if (distance < _range)
            {
                _ammo--;
                OnGameEngineEvent(this, GameEngineEventArgs.Damage(_target, new DamageSource() { Magnitude = _power, Type = DamageType.CONCUSSIVE, Origin = Parent.Position })); // power is not attenuated because there is no drag.
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
            return WeaponType.MASS_DRIVER;
        }

        public void Lock(IDamageableEntity target)
        {
            _target = target;
        }
    }
}
