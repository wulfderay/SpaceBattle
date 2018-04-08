
namespace Spacebattle.entity.parts.Weapon
{
    //Todo: model reloading
    public interface IWeapon : IShipPart
    {
        void FireAt(IDamageableEntity target);
    }
}
