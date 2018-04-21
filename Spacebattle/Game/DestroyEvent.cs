using System;
using Spacebattle.entity;

namespace Spacebattle.Game
{
    public class DestroyEvent: GameEngineEventArgs
    {
        public DestroyEvent() { Type = GameEngineEventType.DESTROYED; }
        public IEntity Entity { get; internal set; }

    }
}