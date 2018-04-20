using System;


namespace Spacebattle.GameEngine
{
    public class GameEngineEventArgs:EventArgs
    {
        public enum GameEngineEventType
        {
            SPAWN = 0,
            DAMAGE
        }
        public GameEngineEventType EventType;

    }
}
