using Spacebattle.Game;

namespace Spacebattle.entity
{
    public interface IGameEntity : IDamageableEntity, IUpdateable, IBehave, IGameEngineEventProvider
    {
    }
}
