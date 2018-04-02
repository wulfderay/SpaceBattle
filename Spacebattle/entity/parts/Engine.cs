

namespace Spacebattle.entity.parts
{
    /// <summary>
    /// Engined provide thrust, but they don't control which direction they are pointed. that's the ship's job.
    /// </summary>
    public class Engine : ShipPart
    {
        
        float _power;

        public Engine(string name, float maxHealth, float mass, float upkeepCost, float power) : base(name, maxHealth, mass, upkeepCost)
        {
            _power = power;
        }

        /// <summary>
        /// returns the power of the engine given some throttle percentage
        /// </summary>
        /// <returns></returns>
        public float Throttle(float percent)
        {
            //TODO: Model heat.... probably add heat for high percent throttle and when engine is damaged... 
            return (_power * _currentHealth/_maxHealth) * percent;
        }

        public override string ToString()
        {
            return "["+_name+" H:" + _currentHealth + "/" + _maxHealth + " P:" + _power + "]";
        }

        public float getMaxForce()
        {
            return (_power * _currentHealth / _maxHealth);
        }
    }
}
