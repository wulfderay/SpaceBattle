

using Spacebattle.entity.parts.Weapon;

namespace Spacebattle.orders
{
    public class FireOrder:Order
    {
        public string WeaponType { get; set; }
        public string WeaponName { get; set; }
    }
}
