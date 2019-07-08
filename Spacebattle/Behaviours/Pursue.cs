using Spacebattle.entity;
using Spacebattle.Entity;
using Spacebattle.orders;

namespace Spacebattle.Behaviours
{
    class Pursue: IBehaviour
    {
        private float _maxThrottle;
        private IControllableEntity _parent;
        private float _preferredDistance;
        private IEntity _target;

        public Pursue(IControllableEntity parent, IEntity target, float preferredDistance, float maxThrottle)
        {
            _parent = parent;
            _target = target;
            _preferredDistance = preferredDistance;
            _maxThrottle = maxThrottle;
        }

        public Order GetNextOrder()
        {
            if ( _parent.Position.DistanceTo(_target.Position) > _preferredDistance)
            {
                // head toward the target and/or speed up
                return Order.SetCourse(_parent.Position.DirectionInDegreesTo(_target.Position), _maxThrottle);
            }
            if (_parent.Position.DistanceTo(_target.Position) < _preferredDistance)
            {
                return Order.SetCourse(_target.Position.DirectionInDegreesTo(_parent.Position), _maxThrottle);
            }
            return Order.NullOrder();
        }
    }
}
