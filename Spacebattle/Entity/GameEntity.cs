using System;
using Spacebattle.physics;
using Spacebattle.Game;
using Spacebattle.Damage;
using Spacebattle.Behaviours;
using System.Collections.Generic;
using Spacebattle.orders;

namespace Spacebattle.entity
{
    public abstract class GameEntity : IGameEntity
    {

        public event EventHandler<GameEngineEventArgs> GameEngineEventHandler;

        public Vector2d Position { get; set; }

        public float Orientation { get; set; } // ideally in radians. but we also need to know the current velocity in xy or xtheta

        public Vector2d Velocity { get; set; } // where x is magnitude and y is direction // or is that what we want?

        public float Mass { get; set; }

        public string Name { get; set; }

        public int Team { get; set; }

        private List<IBehaviour> _behaviours = new List<IBehaviour>();

        public void ApplyImpulse( Vector2d force)
        {
            // F = ma
            // a = f/m
            var accel = force / Mass;
            Velocity += accel;
            //Rounding to account for floating point errors
            if (Math.Abs(Velocity.Magnitude) < 1)
                Velocity = Vector2d.Zero;
        }

        public void DoPhysicsStep()
        {
            Position += Velocity; // no drag in space. 
        }

        protected void OnGameEngineEvent(object sender, GameEngineEventArgs e)
        {
            GameEngineEventHandler?.Invoke(sender, e);
        }

        public abstract void Damage(DamageSource damage);


        public abstract bool IsDestroyed();

        public abstract void Update(uint roundNumber);

        public void AddBehaviour(IBehaviour behaviour)
        {
            _behaviours.Add(behaviour);
        }

        public void RemoveBehaviour(IBehaviour behaviour)
        {
            if (_behaviours.Contains(behaviour))
                _behaviours.Remove(behaviour);
        }

        public Order ExecuteBehaviours()
        {
            foreach (var behaviour in _behaviours)
            {
                var order = behaviour.GetNextOrder();
                if (order.Type != Order.OrderType.NULL_ORDER)
                    return order;
            }
            return Order.NullOrder();

        }
    }
}
