
namespace Spacebattle.entity.parts
{
    public class Reactor : ShipPart
    {
        float _maxPower;

        public Reactor(string name, float maxHealth, float mass, float upkeepCost, float maxPower): base(name, maxHealth, mass, upkeepCost)
        {
            _maxPower = maxPower;
        }

        public float Produce()
        {
            return _maxPower * (_currentHealth / _maxHealth);
        }

        public override string ToString()
        {
            return "[" + _name + " H:" + _currentHealth + "/" + _maxHealth + " P:" + Produce()+"/"+_maxPower+ "]";
        }

    }
}
