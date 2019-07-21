using System;

namespace Spacebattle.Game
{
    public interface IGameEngineEventProvider
    {
        event EventHandler<GameEngineEventArgs> GameEngineEventHandler;
    }
}
