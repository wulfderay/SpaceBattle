﻿using Spacebattle.Damage;
using Spacebattle.Game;
using System;

namespace Spacebattle.entity.parts
{
    public class CrewDeck :ShipPart
    {
        uint _crew;
        float _repairRate;
        float _upkeepCostPerCrew;

        public CrewDeck(string name, float maxHealth, float mass, float upkeepCostPerCrew, uint crewCompliment, float repairRate) : base(name, maxHealth, mass, upkeepCostPerCrew)
        {
            _crew = crewCompliment;
            _repairRate = repairRate;
            _upkeepCostPerCrew = upkeepCostPerCrew;
        }

        public new float GetUpkeepCost()
        {
            return _upkeepCostPerCrew * _crew;
        }
        public new void Update(uint roundNumber)
        {
            base.Update(roundNumber);
            if (Parent.ConsumePower(GetUpkeepCost()) < GetUpkeepCost())
            {
                // ohh shit we are out of power. Life support will fail!
                if (_crew > 0)
                    OnFlavourText(_name, "Loss of power caused life support failure on the " + _name + ". " + _crew + " souls lost.");
                _crew = 0;
            }
            
        }

        public override void Damage(DamageSource damage)
        {
            base.Damage(damage);
            if ( _currentHealth <= 0)
            {
                if ( _crew > 0)
                    OnFlavourText(_name, "Life support failed on the " + _name+". "+_crew+" souls lost.");
                _currentHealth = 0;
                _crew = 0;
                return;
            }
            var crewKilled = (uint) Math.Min((GameEngine.Random((int)damage.Magnitude / 3)), _crew);//kill up  to 1/3* damage crew
            _crew -= crewKilled;
            if (_crew < 0)
                _crew = 0;
            OnFlavourText(_name, crewKilled + " crew killed.");
            
        }

        public void PerformRepair(IShipPart part)
        {
            part.Repair(_crew * _repairRate);
        }

        public uint GetCrew()
        {
            return _crew;
        }

        public override string ToString()
        {
            return "["+_name+" H:"+_currentHealth+" C:" + _crew+ " R:" + _crew * _repairRate +" ]";
        }

    }
}
