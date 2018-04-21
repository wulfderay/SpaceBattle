using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spacebattle.Game
{
    public interface IGameEngineEventProvider
    {
        event EventHandler<GameEngineEventArgs> GameEngineEventHandler;
    }
}
