using Spacebattle.physics;
namespace Spacebattle.entity
{
    public class Entity
    {


        public Vector2d Position; 

        public float Orientation { get; set; } // ideally in radians. but we also need to know the current velocity in xy or xtheta

        public Vector2d Velocity; // where x is magnitude and y is direction // or is that what we want?

        public float Mass { get; protected set; }

        public float DistanceTo(Entity other)
        {
            return Position.DistanceTo(other.Position);
        }

        public float DirectionInDegreesTo(Entity other)
        {
            return Position.DirectionInDegreesTo(other.Position);
        }

        public void ApplyImpulse( Vector2d force)
        {
            // F = ma
            // a = f/m
            var accel = force / Mass;
            Velocity += accel;
        }
    }
}
