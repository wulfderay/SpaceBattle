using Spacebattle.entity;

namespace Spacebattle.Game
{
    public class SpawnEvent: GameEngineEventArgs
    {
        public SpawnEvent() { Type = GameEngineEventType.SPAWN; }
        public IGameEntity Entity { get; internal set; } // we might want to pass in a type or something instead of the entity itself.
    }
}