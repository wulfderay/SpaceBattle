using Spacebattle.entity;
using Spacebattle.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public void Execute()
        {
            if ( _parent.Position.DistanceTo(_target.Position) > _preferredDistance)
            {
                // head toward the target and/or speed up
                _parent.SetCourse(_parent.Position.DirectionInDegreesTo(_target.Position));
                _parent.SetThrottle(_maxThrottle); // just punch it, I guess.
                return;
            }
            if (_parent.Position.DistanceTo(_target.Position) < _preferredDistance)
            {
                _parent.SetCourse(_target.Position.DirectionInDegreesTo(_parent.Position)); // opposite direction.
                _parent.SetThrottle(_maxThrottle); // just punch it, I guess.
                return;
            }

        }
    }
}
