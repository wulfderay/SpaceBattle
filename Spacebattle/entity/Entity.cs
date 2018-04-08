using System;
using Spacebattle.physics;
namespace Spacebattle.entity
{
    public class Entity : IEntity
    {


        public Vector2d Position { get; set; }

        public float Orientation { get; set; } // ideally in radians. but we also need to know the current velocity in xy or xtheta

        public Vector2d Velocity { get; set; } // where x is magnitude and y is direction // or is that what we want?

        public float Mass { get; set; }

        public float DistanceTo(IEntity other)
        {
            return Position.DistanceTo(other.Position);
        }

        public float DirectionInDegreesTo(IEntity other)
        {
            return Position.DirectionInDegreesTo(other.Position);
        }

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
    }
}
