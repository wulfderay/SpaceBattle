using Spacebattle.physics;

namespace Spacebattle.entity
{
    public interface IEntity
    {
        float Mass { get; set; }
        float Orientation { get; set; }
        Vector2d Position { get; set; }
        Vector2d Velocity { get; set; }

        void ApplyImpulse(Vector2d force);
        float DirectionInDegreesTo(IEntity other);
        float DistanceTo(IEntity other);
        void DoPhysicsStep();
    }
}