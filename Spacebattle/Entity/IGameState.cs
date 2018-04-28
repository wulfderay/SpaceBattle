using System.Collections.Generic;

namespace Spacebattle.entity
{
    public interface IGameState
    {
        IEnumerable<IDamageableEntity> GetDamageableEntities();
    }
}