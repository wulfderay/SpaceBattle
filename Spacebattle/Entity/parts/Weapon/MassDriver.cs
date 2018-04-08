using Spacebattle.entity.parts.Weapon;
using Spacebattle.entity;
using Spacebattle.Damage;

namespace Spacebattle.Entity.parts.Weapon
{
    class MassDriver : ShipPart, IWeapon
    {
        uint _ammo;
        float _power;
        float _range;
        public MassDriver(string name, float maxHealth, float mass, float upkeepCost, float power, uint ammo, float range) : base(name, maxHealth, mass, upkeepCost)
        {
            _ammo = ammo;
            _power = power;
            _range = range;
        }

        public void FireAt(IDamageableEntity target)
        {
            if (IsDestroyed())
                return;
            if ( _ammo <= 0)
            {
                OnFlavourText(_name, "No more ammo!");
                return;
            }
            var distance = Parent.DistanceTo(target);
            if (distance < _range)
            {
                _ammo--;
                target.Damage(new DamageSource() { Magnitude = _power, Type = DamageType.CONCUSSIVE, Origin = Parent.Position }); // power is not attenuated because there is no drag.
            }
            else
                OnFlavourText(_name, "Target was too far away to hit!");
        }
    }
}
