using Spacebattle.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spacebattle.Behaviours
{
    class Pursue: IBehaviour
    {
        private float _maxSpeed;
        private IEntity _parent;
        private float _preferredDistance;
        private IEntity _target;

        public Pursue(IEntity parent, IEntity target, float preferredDistance, float maxSpeed)
        {
            _parent = parent;
            _target = target;
            _preferredDistance = preferredDistance;
            _maxSpeed = maxSpeed;
        }

        public void Update(uint roundNumber)
        {
            if ( _parent.Position.DistanceTo(_target.Position) > _preferredDistance)
            {
                // head toward the target and/or speed up
                
                return;
            }
            if (_parent.Position.DistanceTo(_target.Position) < _preferredDistance)
            {
                // back off a bit
                return;
            }

        }
    }
}
