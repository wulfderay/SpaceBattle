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

        private const uint LOADING_ROUNDS = 3;
        public uint TurnsUntilLoaded { get; private set; } = 0;
        public uint TotalLoadingTurns { get => LOADING_ROUNDS; }

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
            if ( TurnsUntilLoaded > 0)
            {
                OnFlavourText(_name, "Weapon not fully charged!");
                return;
            }
            var distance = Parent.Position.DistanceTo(_target.Position);
            if (distance < _range)
            {
                TurnsUntilLoaded = TotalLoadingTurns;
                OnGameEngineEvent(this, GameEngineEventArgs.Damage(_target, new DamageSource() { Magnitude = (_power - (_power * distance / _range)) / 2, DamageType = DamageType.DRAINING, Origin = Parent.Position }));
                OnGameEngineEvent(this, GameEngineEventArgs.Damage(_target, new DamageSource() { Magnitude = _power / 2, DamageType = DamageType.FIRE, Origin = Parent.Position }));
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

        public bool IsReadyToFire()
        {
            return TurnsUntilLoaded == 0;
        }

        public void Lock(IDamageableEntity target)
        {
            _target = target;
        }

        public bool TargetIsInRange()
        {
            return _target != null && Parent.Position.DistanceTo(_target.Position) < _range;
        }

        public new void Update(uint roundNumber)
        {
            base.Update(roundNumber);
            if (IsDestroyed())
                return;
            if (Parent.ConsumePower(GetUpkeepCost()) < GetUpkeepCost())
            {

                OnFlavourText(_name, $"Not enough power for {_name}. Cannot load.");
                return;

            }
            if ( TurnsUntilLoaded> 0)
                TurnsUntilLoaded--;
            
        }

        public static PlasmaBolt StandardPlasmaBolt()
        {
            return new PlasmaBolt("Plasmabolt", 100, 10, 75, 100, 750);
        }
    }
}
