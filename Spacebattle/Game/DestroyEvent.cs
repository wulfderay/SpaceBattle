using Spacebattle.entity;

namespace Spacebattle.Game
{
    public class DestroyEvent: GameEngineEventArgs
    {
        public DestroyEvent() { Type = GameEngineEventType.DESTROYED; }
        public IGameEntity Entity { get; internal set; }

    }
}