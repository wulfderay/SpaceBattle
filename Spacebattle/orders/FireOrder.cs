

using Spacebattle.entity.parts.Weapon;

namespace Spacebattle.orders
{
    public class FireOrder:Order
    {
        public WeaponType? WeaponType { get; set; }
        public string WeaponName { get; set; }
    }
}
