
using Spacebattle.Damage;
using System;

namespace Spacebattle.entity.parts
{
    public class Shield : ShipPart
    {
        float _currentAbsorbtion;
        float _maxAbsorbtion;
        float _regenRate;
        float _powerPerRegenUnit;
        float _arcWidthDegrees;
        float _baseAngle;

        public ShieldStatus Status { get; set; } = ShieldStatus.RAISED;
        public Tuple<float, float> Strength { get { return new Tuple<float, float>(Status == ShieldStatus.LOWERED? 0 :_currentAbsorbtion, _maxAbsorbtion); } }

        public Shield(string name, float maxHealth, float mass,float upkeepCost,float maxAbsorbtion, float regenRate, float powerPerRegenUnit, float arcWidthDegrees, float baseAngle): base(name, maxHealth, mass, upkeepCost)
        {
            _maxAbsorbtion= _currentAbsorbtion = maxAbsorbtion;
            _regenRate = regenRate;
            _powerPerRegenUnit = powerPerRegenUnit;
            _arcWidthDegrees = arcWidthDegrees;
            _baseAngle = baseAngle;
        }
        public float Regen(float powerBudget)
        {

            // we heal the least amount given  a) what's needed, b)what our regen rate allows, and c) how much power we have available.
            var amountToRegen = Math.Min(Math.Min(_maxAbsorbtion - _currentAbsorbtion, _regenRate * (_currentHealth / _maxHealth)), powerBudget/_powerPerRegenUnit);
            var powerConsumed = amountToRegen * _powerPerRegenUnit;

            _currentAbsorbtion += amountToRegen;
            if (_currentAbsorbtion > _maxAbsorbtion)
                _currentAbsorbtion = _maxAbsorbtion;

            return powerBudget - powerConsumed;
        }

        public new void Update(uint roundNumber)
        {
            base.Update(roundNumber);
            if (Status == ShieldStatus.LOWERED)
                return;
            if (Parent.ConsumePower(GetUpkeepCost()) < GetUpkeepCost())
            {

                OnFlavourText(_name, $"Not enough power for {_name}. Lowering Shield..");
                Status = ShieldStatus.LOWERED;

            }

        }
        public new float GetUpkeepCost()
        {
            return Status == ShieldStatus.LOWERED ? 0 : _upkeepCost;
        }

        /// <summary>
        /// Absorb some of the damage from a shot.
        /// </summary>
        /// <param name="damage"></param>
        /// <returns>the residual damage that was not absorbed.</returns>
        public DamageSource Absorb(DamageSource damage)
        {
            if (Status == ShieldStatus.LOWERED)
                return damage;
            if (!shieldInterceptsDamage(damage))
                return damage;
            OnFlavourText(_name, "Shield Hit!");
            var residualDamage = damage.Magnitude;
            var modifier = 1f;
            if (damage.DamageType == DamageType.DRAINING)
                modifier = 2f;
            if (damage.DamageType == DamageType.CONCUSSIVE)
                modifier = 0.5f;
            // apply a modifier to the damage to the sheild, but remove that modifire before it gets to the hull.

            _currentAbsorbtion -= damage.Magnitude * modifier;
            if (_currentAbsorbtion < 0)
            {
                var residual = Math.Abs(_currentAbsorbtion)/modifier;
                _currentAbsorbtion = 0;
                return new DamageSource { Magnitude = residual, Origin = damage.Origin, DamageType = damage.DamageType };
            }
            return new DamageSource { Magnitude = 0, Origin = damage.Origin, DamageType = damage.DamageType };
        }

        private bool shieldInterceptsDamage(DamageSource damage)
        {
            // adding 360 and % 720 will get rid of the transition from 359 to 0
            var direction = Parent.Position.DirectionInDegreesTo(damage.Origin) + 360;
            direction %= 720;
            var baseAngle = (((_baseAngle+ Parent.Orientation)%360) + 360 ) % 720;
            return (direction >= baseAngle && direction <= baseAngle + _arcWidthDegrees); 
        }

        public override string ToString()
        {
            return "["+_name+" H:" + _currentHealth + "/" + _maxHealth + " A:" + _currentAbsorbtion + "/" + _maxAbsorbtion + "]";
        }

        // fast but expensive and fragile
        public static Shield FastRegenshield(string name, float arcWidthDegrees, float baseAngle)
        {
            return new Shield(name, 25, 10, 75, 400, 10f, 5, arcWidthDegrees, baseAngle);
        }

        // slow but efficient
        public static Shield Bigshield(string name,float arcWidthDegrees, float baseAngle)
        {
            return new Shield(name, 45, 15, 125, 1000, 5f, 2, arcWidthDegrees, baseAngle);
        }
    }

    public enum ShieldStatus
    {
        RAISED = 0,
        LOWERED
    }
}
