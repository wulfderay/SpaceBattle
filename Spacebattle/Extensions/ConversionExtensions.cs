using System;

namespace Spacebattle.Extensions
{
    public static class ConversionExtensions
    {
        public static double ToRadians(this float angleInDegrees)
        {
            return (Math.PI / 180) * angleInDegrees;
        }

        public static double ToRadians(this double angleInDegrees)
        {
            return (Math.PI / 180) * angleInDegrees;
        }

        public static double ToDegrees(this float angleInRadians)
        {
            return ( 180/Math.PI) * angleInRadians;
        }

        public static double ToDegrees(this double angleInRadians)
        {
            return (180 / Math.PI) * angleInRadians;
        }
    }
}
