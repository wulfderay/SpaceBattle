
using System;
namespace Spacebattle.orders
{
    public class Order
    {
        public enum OrderType { NULL_ORDER = 0,SET_COURSE, SET_THROTTLE, FIRE, LOCK, ALL_STOP};

        public OrderType Type;

        protected Order() // don't allow creation but by the factory methods.
        {

        }

        public static Order NullOrder()
        {
            return new Order() { Type = OrderType.NULL_ORDER };
        }

        public static SetCourseOrder SetCourse(float angleInDegrees)
        {
            return new SetCourseOrder { Type = OrderType.SET_COURSE, AngleInDegrees = angleInDegrees};
        }

        public static SetThrottleOrder SetThrottle(float percent)
        {
            if (percent > 100 || percent < 0)
                throw new ArgumentOutOfRangeException("You must specify a percent between 0 and 100. You specified " + percent);
            return new SetThrottleOrder { Type = OrderType.SET_THROTTLE, ThrottlePercent = percent };
        }

        public static Order Fire() //TODO: fire a specific weapon
        {
            return new Order { Type = OrderType.FIRE };
        }

        public static Order AllStop() //TODO: fire a specific weapon
        {
            return new Order { Type = OrderType.ALL_STOP};
        }

        public static LockOrder Lock(string shipName)
        {
            return new LockOrder { Type = OrderType.LOCK, ShipToLockOn = shipName };
        }
    }
}
