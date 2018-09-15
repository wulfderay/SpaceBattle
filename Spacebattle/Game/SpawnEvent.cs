using Spacebattle.entity;
using Spacebattle.physics;

namespace Spacebattle.Game
{
    public class SpawnEvent: GameEngineEventArgs
    {
        public SpawnEvent() { Type = GameEngineEventType.SPAWN; }
        public Vector2d Where { get; internal set; }
        public IGameEntity Entity { get; internal set; } // we might want to pass in a type or something instead of the entity itself.
    }
}