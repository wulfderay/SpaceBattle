using Spacebattle.entity;
using Spacebattle.entity.parts.Weapon;
using System.Collections.Generic;

namespace Spacebattle.Entity
{
    public interface IControllableEntity: IDamageableEntity, IUpdateable
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
        List<IDamageableEntity> GetVisibleEntites(); // this will end up relying on scanners... that's why it's here.
        
    }
}
