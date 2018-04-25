using Spacebattle.entity.parts.Weapon;

namespace Spacebattle.Entity
{
    interface IControllableEntity:IDamageableEntity
    {
        void AllStop();
        float GetMaxAcceleration(); // is this really needed?
        void LockOn(IDamageableEntity ship);
        void LockOn(IDamageableEntity ship, WeaponType weaponType);
        void LockOn(IDamageableEntity ship, string weaponName);
        void SetCourse(float angle);
        void SetVelocity(float speed);
        void SetThrottle(float percent);
        float GetThrottle();
        void Shoot();
        void Shoot(WeaponType weaponType);
        void Shoot(string weaponName);
    }
}
