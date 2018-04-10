
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

        /// <summary>
        /// Absorb some of the damage from a shot.
        /// </summary>
        /// <param name="damage"></param>
        /// <returns>the residual damage that was not absorbed.</returns>
        public DamageSource Absorb(DamageSource damage)
        {
            if (!shieldInterceptsDamage(damage))
                return damage;
            OnFlavourText(_name, "Shield Hit!");
            var residualDamage = damage.Magnitude;
            var modifier = 1f;
            if (damage.Type == DamageType.DRAINING)
                modifier = 2f;
            if (damage.Type == DamageType.CONCUSSIVE)
                modifier = 0.5f;
            // apply a modifier to the damage to the sheild, but remove that modifire before it gets to the hull.

            _currentAbsorbtion -= damage.Magnitude * modifier;
            if (_currentAbsorbtion < 0)
            {
                var residual = Math.Abs(_currentAbsorbtion)/modifier;
                _currentAbsorbtion = 0;
                return new DamageSource { Magnitude = residual, Origin = damage.Origin, Type = damage.Type };
            }
            return new DamageSource { Magnitude = 0, Origin = damage.Origin, Type = damage.Type };
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
            return new Shield(name, 25, 10, 10, 40, 10f, 5, arcWidthDegrees, baseAngle);
        }

        // slow but efficient
        public static Shield Bigshield(string name,float arcWidthDegrees, float baseAngle)
        {
            return new Shield(name, 45, 15, 2, 100, 5f, 2, arcWidthDegrees, baseAngle);
        }
    }
}
