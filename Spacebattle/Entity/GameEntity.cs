using System;
using Spacebattle.physics;
using Spacebattle.Game;
using Spacebattle.Damage;

namespace Spacebattle.entity
{
    public class GameEntity : IGameEntity
    {

        public event EventHandler<GameEngineEventArgs> GameEngineEventHandler;

        public Vector2d Position { get; set; }

        public float Orientation { get; set; } // ideally in radians. but we also need to know the current velocity in xy or xtheta

        public Vector2d Velocity { get; set; } // where x is magnitude and y is direction // or is that what we want?

        public float Mass { get; set; }

        public string Name { get; set; }

        public int Team { get; set; }

        public void ApplyImpulse( Vector2d force)
        {
            // F = ma
            // a = f/m
            var accel = force / Mass;
            Velocity += accel;
            //Rounding to account for floating point errors
            if (Math.Abs(Velocity.Magnitude()) < 1)
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

        public void Damage(DamageSource damage)
        {
            throw new NotImplementedException();
        }

        public bool IsDestroyed()
        {
            throw new NotImplementedException();
        }
    }
}
