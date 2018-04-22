using Spacebattle.entity.parts.Weapon;

namespace Spacebattle.orders
{
    public class LockOrder:Order
    {
        public WeaponType? WeaponType { get; set; }
        public string WeaponName { get; set; }
        public string ShipToLockOn { get; set; }
    }
}