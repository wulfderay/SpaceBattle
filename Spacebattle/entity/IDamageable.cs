
namespace Spacebattle.entity
{
    public interface IDamageable
    {
        /// <summary>
        /// damage this entity
        /// TODO: make this care about angle
        /// </summary>
        /// <param name="damage">the amount of damage to deal</param>
        void Damage(float damage);
        /// <summary>
        /// returns whether the entity is destroyed.
        /// </summary>
        /// <returns></returns>
        bool IsDestroyed();
    }
}
