
using Spacebattle.entity.parts.Weapon;
using System;
namespace Spacebattle.orders
{
    public class Order
    {
        public enum OrderType { NULL_ORDER = 0,HELM, FIRE, LOCK, ALL_STOP, SCAN };

        public OrderType Type;

        protected Order() // don't allow creation but by the factory methods.
        {

        }

        public static Order NullOrder()
        {
            return new Order() { Type = OrderType.NULL_ORDER };
        }

        public static HelmOrder SetCourse(float angleInDegrees, float? percent = null)
        {
            return new HelmOrder { Type = OrderType.HELM, AngleInDegrees = angleInDegrees, ThrottlePercent = percent};
        }

        public static HelmOrder SetThrottle(float percent)
        {
            if (percent > 100 || percent < 0)
                throw new ArgumentOutOfRangeException("You must specify a percent between 0 and 100. You specified " + percent);
            return new HelmOrder { Type = OrderType.HELM, ThrottlePercent = percent };
        }

        public static FireOrder Fire(WeaponType? weaponType=null, string weaponName=null) //TODO: fire a specific weapon
        {
            return new FireOrder { Type = OrderType.FIRE, WeaponType = weaponType, WeaponName = weaponName};
        }

        public static Order AllStop() //TODO: fire a specific weapon
        {
            return new Order { Type = OrderType.ALL_STOP};
        }

        public static LockOrder Lock(string shipName, WeaponType? weaponType = null, string weaponName = null)
        {
            return new LockOrder { Type = OrderType.LOCK, ShipToLockOn = shipName ,  WeaponType = weaponType, WeaponName = weaponName };
        }

        internal static ScanOrder Scan(string shipName)
        {
            return new ScanOrder
            {
                Type = OrderType.SCAN,
                ShipToScan = shipName,
            };
        }
    }
}
