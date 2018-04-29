

using System;
using System.Collections.Generic;
using Spacebattle.Behaviours;
using Spacebattle.Damage;
using Spacebattle.entity;
using Spacebattle.entity.parts.Weapon;
using Spacebattle.Game;
using Spacebattle.physics;

namespace Spacebattle.Entity.parts.Weapon
{
    class Torpedo : GameEntity, IControllableEntity, IBehave
    {
        // this isn't quite right... A torpedo needs to know it's target, sure.. but the target maybe? should be able to be changed.
        // it probably doesn't belong in the constructor.
        // It _does_ need a payload and health and speed and other fun stuff like that...

        private IDamageableEntity target;
        private bool _isDestroyed;
        private List<IBehaviour> _behaviours;
        private float _throttle;

        public Torpedo(IDamageableEntity target)
        {
            this.target = target;
            AddBehaviour(new Pursue(this, target, 0, 100));

        }
        public void AllStop()
        {
            Velocity = Vector2d.Zero;
            SetThrottle(0);
        }

        public void Damage(DamageSource damage) // any damage at this point should destroy it.
        {
            _isDestroyed = true;
            OnGameEngineEvent(this, GameEngineEventArgs.Destroy(this));
        }

        public float GetMaxAcceleration()
        {
            throw new NotImplementedException();
        }

        public float GetThrottle()
        {
            return _throttle;
        }

        public List<IDamageableEntity> GetVisibleEntites()
        {
            return new List<IDamageableEntity>() { target }; // no sensors yet I guess.
        }

        public bool IsDestroyed()
        {
            return _isDestroyed;
        }

        public void SetCourse(float angle)
        {
            Orientation = angle;
        }

        public void SetThrottle(float percent)
        {
            _throttle = percent;
        }

        public void Update(uint roundNumber)
        {
          // dunno.
          // I guess if it's near enough to the target it will blow up and emit damage... 
        }

        public void AddBehaviour(IBehaviour behaviour)
        {
            _behaviours.Add(behaviour);
        }

        public void RemoveBehaviour(IBehaviour behaviour)
        {
            if (_behaviours.Contains(behaviour))
                _behaviours.Remove(behaviour);
        }

        public void ExecuteBehaviours()
        {
            foreach (var behaviour in _behaviours)
                behaviour.Execute();
        }



        

    }
}
