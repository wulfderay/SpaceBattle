
using Spacebattle.Damage;
using Spacebattle.Game;

namespace Spacebattle.entity.parts.Weapon
{
    class Lance:ShipPart,IWeapon
    {
        float _power;
        float _range;
        private IDamageableEntity _target;
        private const uint LOADING_ROUNDS = 5;
        public uint TurnsUntilLoaded { get; private set; } = 0;
        public uint TotalLoadingTurns { get => LOADING_ROUNDS; }

        public Lance(string name, float maxHealth, float mass, float upkeepCost, float power, float range): base(name, maxHealth, mass, upkeepCost)
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
            if (TurnsUntilLoaded > 0)
            {
                OnFlavourText(_name, "Weapon not fully charged!");
                return;
            }
            var distance = Parent.Position.DistanceTo(_target.Position);
            if (distance < _range)
            {
                TurnsUntilLoaded = TotalLoadingTurns;
                OnGameEngineEvent(this, GameEngineEventArgs.Damage(_target, new DamageSource() { Magnitude = _power - (_power * distance / _range), DamageType = DamageType.PIERCING, Origin = Parent.Position }));
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
            return WeaponType.LANCE;
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
        public new void Update(uint roundNumber)
        {
            base.Update(roundNumber);
            if (IsDestroyed())
                return;

            if (TurnsUntilLoaded > 0)
                TurnsUntilLoaded--;

        }
    }
}
