
using Spacebattle.Damage;
using Spacebattle.Game;

namespace Spacebattle.entity.parts.Weapon
{
    // what I want from this weapon is to directly convert ship energy to damage.
    // in an ideal setup, this would happen by way of a power distribution system under user control.
    class Phaser:ShipPart,IWeapon
    {
        float _power;
        float _range;
        private IDamageableEntity _target;

        public Phaser(string name, float maxHealth, float mass, float upkeepCost, float power, float range): base(name, maxHealth, mass, upkeepCost)
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
            {
                // use power for charge...
                OnGameEngineEvent(this, GameEngineEventArgs.Damage(_target, (new DamageSource() { Magnitude = _power - (_power * distance / _range), DamageType = DamageType.DRAINING, Origin = Parent.Position })));
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
            return WeaponType.PHASER;
        }

        public bool IsReadyToFire()
        {
            return true;
        }

        public void Lock(IDamageableEntity target)
        {
            _target = target;
        }

        public bool TargetIsInRange()
        {
            return _target != null && Parent.Position.DistanceTo(_target.Position) < _range;
        }

        public override string ToString()
        {
            return "[" + _name + " H:" + _currentHealth + "/" + _maxHealth + " P:" + _power + " R:" + _range + "]";
        }
    }
}
