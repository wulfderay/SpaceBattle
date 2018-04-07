using Spacebattle.Damage;
using System;

namespace Spacebattle.entity.parts
{
    public class CrewDeck :ShipPart
    {
        uint _crew;
        float _repairRate;

        public CrewDeck(string name, float maxHealth, float mass, float upkeepCost, uint crewCompliment, float repairRate): base(name, maxHealth, mass, upkeepCost)
        {
            _crew = crewCompliment;
            _repairRate = repairRate;
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

        public static CrewDeck MilitaryDeck()
        {
            return new CrewDeck("Military Deck",100, 200, 20, 200, 0.1f);
        }

        public static CrewDeck PleasureDeck()
        {
            return new CrewDeck("Pleasure Deck", 100, 400, 100, 500, 0.01f);
        }

        public static CrewDeck EngineeringDeck()
        {
            return new CrewDeck("Engineering Deck",75, 200, 30, 100, 0.5f);
        }

        public static CrewDeck Bridge()
        {
            return new CrewDeck("Bridge", 50, 50, 10, 10, 0.0f);
        }
    }
}
