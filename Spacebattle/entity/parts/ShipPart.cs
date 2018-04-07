
using Spacebattle.Damage;
using System;

namespace Spacebattle.entity
{
    public class ShipPart:IShipPart,IDamageable,IFlavourTextProvider
    {
        protected float _currentHealth;
        protected float _maxHealth;
        protected float _mass;
        protected float _upkeepCost;
        protected string _name;

        public event EventHandler<FlavourTextEventArgs> FlavourTextEventHandler;

        public ShipPart(string name, float maxHealth, float mass, float upkeepCost)
        {
            _currentHealth = _maxHealth = maxHealth;
            _mass = mass;
            _upkeepCost = upkeepCost;
            _name = name;
        }

        public virtual void Damage(DamageSource damage)
        {
            if (IsDestroyed())
                return;
            _currentHealth -= damage.Magnitude;
            if (_currentHealth < 0)
                _currentHealth = 0;
            if (IsDestroyed())
                OnFlavourText(_name, _name + " was destroyed!");
        }

        public bool IsDestroyed()
        {
            return _currentHealth <= 0;
        }
        public float GetMass()
        {
            return _mass;
        }
        public float GetHealth()
        {
            return _currentHealth;
        }

        public void Repair(float repairAmount)
        {
            if (IsDestroyed() && repairAmount > 0)
                OnFlavourText(_name, _name + " is functional again!");
            else if (_currentHealth + repairAmount >= _maxHealth)
                OnFlavourText(_name, _name + " is fully repaired!");
            _currentHealth += repairAmount;
            if (_currentHealth > _maxHealth)
                _currentHealth = _maxHealth;
        }

        public float GetMaxHealth()
        {
            return _maxHealth;
        }

        public float GetUpkeepCost()
        {
            if (IsDestroyed())
                return 0;
            return _upkeepCost;
        }

        protected void OnFlavourText(string name, string message)
        {
            FlavourTextEventHandler?.Invoke(this, new FlavourTextEventArgs { name = name, message = message });
        }
    }
}