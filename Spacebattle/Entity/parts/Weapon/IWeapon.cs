
namespace Spacebattle.entity.parts.Weapon
{
    //Todo: model reloading
    public interface IWeapon : IShipPart
    {

        void Fire();
        WeaponType GetWeaponType();
        void Lock(IDamageableEntity target);
        IDamageableEntity GetLockTarget();

    }
}
