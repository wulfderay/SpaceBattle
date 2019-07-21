using Spacebattle.entity.parts.Weapon;
using Spacebattle.entity;
using Spacebattle.Damage;
using Spacebattle.Game;

namespace Spacebattle.Entity.parts.Weapon
{
    class MassDriver : ShipPart, IWeapon
    {
        float _power;
        float _range;
        private IDamageableEntity _target;
        public uint Ammo { get; private set; }

        public MassDriver(string name, float maxHealth, float mass, float upkeepCost, float power, uint ammo, float range) : base(name, maxHealth, mass, upkeepCost)
        {
            Ammo = ammo;
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
            if (Ammo <= 0)
            {
                OnFlavourText(_name, "No more ammo!");
                return;
            }
            var distance = Parent.Position.DistanceTo(_target.Position);
            if (distance < _range)
            {
                Ammo--;
                OnGameEngineEvent(this, GameEngineEventArgs.Damage(_target, new DamageSource() { Magnitude = _power, DamageType = DamageType.CONCUSSIVE, Origin = Parent.Position })); // power is not attenuated because there is no drag.
            }
            else
                OnFlavourText(_name, "Target was too far away to hit!");
        }

        

        public IDamageableEntity GetLockTarget()
        {
            return _target;
        }

        public string GetWeaponType()
        {
            return WeaponType.MASSDRIVER;
        }

        public bool IsReadyToFire()
        {
            return Ammo > 0;
        }

        public void Lock(IDamageableEntity target)
        {
            _target = target;
        }

        public bool TargetIsInRange()
        {
            return _target != null && Parent.Position.DistanceTo(_target.Position) < _range;
        }
    }
}
