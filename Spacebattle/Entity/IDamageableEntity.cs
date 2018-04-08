using Spacebattle.entity;

namespace Spacebattle
{
    /// <summary>
    /// This is just a mushertogetherer so that we can guarantee being able to get the distance to something we want to damage./.. 
    /// Is this a good idea? :shrug:
    /// </summary>
    public interface IDamageableEntity:IDamageable,IEntity
    {
    }
}
