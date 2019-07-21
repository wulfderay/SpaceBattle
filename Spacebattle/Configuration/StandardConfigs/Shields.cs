using Spacebattle.Configuration.Schema;
using System.Collections.Generic;

namespace Spacebattle.Configuration.StandardConfigs
{
    public static class Shields
    {
        // fast but expensive and fragile
        public static ShieldSchema FastRegenshield(string name, float arcWidthDegrees, float baseAngle)
        {
            return new ShieldSchema(name, 100, 10, 75, 400, 10f, 5, arcWidthDegrees, baseAngle);
        }

        // slow but efficient
        public static ShieldSchema Bigshield(string name, float arcWidthDegrees, float baseAngle)
        {
            return new ShieldSchema(name, 200, 15, 125, 1000, 5f, 2, arcWidthDegrees, baseAngle);
        }


        public static List<ShieldSchema> SurroundWithFastShields(int num = 6)
        {
            var result = new List<ShieldSchema>();
            for (int i = 0; i < num; i++)
            {
                result.Add(FastRegenshield("Shield " + (i + 1), 360 / num, 360 / num * i));
            }
            return result;
        }

        public static List<ShieldSchema> SurroundWithBigShields(int num = 6)
        {
            var result = new List<ShieldSchema>();
            for (int i = 0; i < num; i++)
            {
                result.Add(Bigshield("Shield " + (i + 1), 360 / num, 360 / num * i));
            }
            return result;
        }
    }
}
