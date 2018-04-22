using Spacebattle.Game;
using Spacebattle.physics;
using System;

namespace Spacebattle.entity
{
    public interface IEntity: IGameEngineEventProvider
    {
        float Mass { get; set; }
        float Orientation { get; set; }
        Vector2d Position { get; set; }
        Vector2d Velocity { get; set; }
        string Name { get;  set; }

        void ApplyImpulse(Vector2d force);
        float DirectionInDegreesTo(IEntity other);
        float DistanceTo(IEntity other);
        void DoPhysicsStep();
        
    }
}