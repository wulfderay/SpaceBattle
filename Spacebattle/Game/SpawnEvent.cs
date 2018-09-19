using Spacebattle.entity;
using Spacebattle.physics;

namespace Spacebattle.Game
{
    public class SpawnEvent: GameEngineEventArgs
    {
        public SpawnEvent() { Type = GameEngineEventType.SPAWN; }
        public Vector2d Position { get; internal set; }
        public float Orientation { get; internal set; }
        public Vector2d Velocity { get; internal set; }
        public IGameEntity Entity { get; internal set; } // we might want to pass in a type or something instead of the entity itself.
    }
}