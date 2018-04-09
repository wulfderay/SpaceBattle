
namespace Spacebattle.entity.parts.Weapon
{
    //Todo: model reloading
    public interface IWeapon : IShipPart
    {
        /// <summary>
        /// Do some damage to some entity,
        /// TODO: make htis interact with the physics system so that you can, for example, accidentally hit a ship between you and your target
        /// </summary>
        /// <param name="target"></param>
        void FireAt(IDamageableEntity target);
    }
}
