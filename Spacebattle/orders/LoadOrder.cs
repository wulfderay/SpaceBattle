using Spacebattle.entity.parts.Weapon;
using Spacebattle.orders;

namespace Spacebattle.Orders
{
    public class LoadOrder: Order
    {
        public string WeaponType { get; set; }
        public string WeaponName { get; set; }
    }
}
