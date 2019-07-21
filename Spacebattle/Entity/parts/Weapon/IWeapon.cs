
namespace Spacebattle.entity.parts.Weapon
{
    //Todo: model reloading
    public interface IWeapon : IShipPart
    {

        void Fire();
        string GetWeaponType();
        void Lock(IDamageableEntity target);
        IDamageableEntity GetLockTarget();
        bool TargetIsInRange();
        bool IsReadyToFire();

    }
}
