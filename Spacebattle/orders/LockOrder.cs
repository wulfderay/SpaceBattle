using Spacebattle.entity.parts.Weapon;

namespace Spacebattle.orders
{
    public class LockOrder:Order
    {
        public string WeaponType { get; set; }
        public string WeaponName { get; set; }
        public IDamageableEntity Target { get; set; }
    }
}