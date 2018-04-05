
using System;

namespace Spacebattle.entity.parts
{
    public class Shield : ShipPart
    {
        float _currentAbsorbtion;
        float _maxAbsorbtion;
        float _regenRate;
        float _powerPerRegenUnit;

        public Shield(string name, float maxHealth, float mass,float upkeepCost,float maxAbsorbtion, float regenRate, float powerPerRegenUnit): base(name, maxHealth, mass, upkeepCost)
        {
            _maxAbsorbtion= _currentAbsorbtion = maxAbsorbtion;
            _regenRate = regenRate;
            _powerPerRegenUnit = powerPerRegenUnit;
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
        public float Absorb(float damage)
        {
            _currentAbsorbtion -= damage;
            if (_currentAbsorbtion < 0)
            {
                var residual = Math.Abs(_currentAbsorbtion);
                _currentAbsorbtion = 0;
                return residual;
            }
            return 0;
        }

        public override string ToString()
        {
            return "["+_name+" H:" + _currentHealth + "/" + _maxHealth + " A:" + _currentAbsorbtion + "/" + _maxAbsorbtion + "]";
        }

        // fast but expensive and fragile
        public static Shield FastRegenshield()
        {
            return new Shield("Fast shield",50, 50, 20, 100, 20f, 5);
        }

        // slow but efficient
        public static Shield Bigshield()
        {
            return new Shield("Big shield",100, 100, 10, 200, 15f, 2);
        }
    }
}
