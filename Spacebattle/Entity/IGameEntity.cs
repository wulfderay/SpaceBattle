using System;
using Spacebattle.physics;
using Spacebattle.Game;

namespace Spacebattle.entity
{
    public interface IGameEntity : IDamageableEntity, IGameEngineEventProvider
    {
    }
}
