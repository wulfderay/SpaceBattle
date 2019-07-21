

namespace Spacebattle.entity.parts
{
    /// <summary>
    /// Engines provide thrust, but they don't control which direction they are pointed. that's the ship's job.
    /// </summary>
    public class Engine : ShipPart
    {
        
        float _thrustPower;
        float _currentThrottle;

        public Engine(string name, float maxHealth, float mass, float upkeepCost, float thrustPower) : base(name, maxHealth, mass, upkeepCost)
        {
            _thrustPower = thrustPower;
        }

        /// <summary>
        /// returns the thrustPower of the engine given some throttle percentage
        /// TODO: keep the throttle state on the engine
        /// </summary>
        /// <returns></returns>
        public float Throttle(float percent)
        {
            _currentThrottle = percent;
            if ( Parent.Power > GetUpkeepCost())
            //TODO: Model heat.... probably add heat for high percent throttle and when engine is damaged... 
                return (_thrustPower * _currentHealth/_maxHealth) * percent;
            return 0;
        }

        public new float GetUpkeepCost()
        {
            var upkeep = _upkeepCost + (_thrustPower * _currentThrottle * .01f);
            return upkeep;
        }
        public new void Update(uint roundNumber)
        {
            base.Update(roundNumber);
            if (Parent.ConsumePower(GetUpkeepCost()) < GetUpkeepCost())
            {
                
                OnFlavourText(_name, $"Not enough power for the {_name}. Shutting down engine.");
                
            }

        }

        public override string ToString()
        {
            return "["+_name+" H:" + _currentHealth + "/" + _maxHealth + " P:" + _thrustPower +" Mf:"+getMaxForce() +"]";
        }

        public float getMaxForce()
        {
            return (_thrustPower * _currentHealth / _maxHealth);
        }
    }
}
