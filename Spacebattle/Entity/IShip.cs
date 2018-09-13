using Spacebattle.entity.parts.Weapon;
using Spacebattle.Entity;
using System.Collections.Generic;

namespace Spacebattle.entity
{
    public interface IShip : IGameEntity, IControllableEntity, IFlavourTextProvider, IBehave
    {

        IGameState gameState { get; set; }
        void LockOn(IDamageableEntity ship);
        void LockOn(IDamageableEntity ship, WeaponType weaponType);
        void LockOn(IDamageableEntity ship, string weaponName);
        void Shoot();
        void Shoot(WeaponType weaponType);
        void Shoot(string weaponName);
        List<IDamageableEntity> GetVisibleEntites(); // this will end up relying on scanners... that's why it's here.
    }
}