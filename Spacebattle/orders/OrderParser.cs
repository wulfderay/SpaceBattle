using System;

namespace Spacebattle.orders
{
    class OrderParser
    {
        /// <summary>
        /// Turns plain text into orders.
        /// TODO: Make this actually do some light nlp
        /// </summary>
        /// <param name="plaintext"></param>
        /// <returns></returns>
        public static Order ParseOrder(string plaintext)
        {
            if (string.IsNullOrEmpty(plaintext))
                return Order.NullOrder();

            var tokens = plaintext.Split(' ');

            switch (tokens[0].ToLower())
            {
                case "he":
                case "helm":
                case "setcourse":
                    if ( tokens.Length > 1)
                        return parseAsSetCourse(tokens);
                    break;
                case "setthrottle":
                    if (tokens.Length > 1)
                        return parseAsSetThrottle(tokens);
                    break;
                case "lock":
                    if (tokens.Length > 2)
                    {
                        return parseAsLock(tokens);
                    }
                    if (tokens.Length > 1)
                        return Order.Lock(tokens[1]);
                    break;
                case "fire":
                    if (tokens.Length > 1)
                    {
                        return parseAsFire(tokens);
                    }
                    
                    return Order.Fire();
                case "scan":
                    if (tokens.Length > 1)
                        return Order.Scan(tokens[1]);
                    break;
                case "allstop":
                    return Order.AllStop();

            }
            return Order.NullOrder();
        }


        private static Order parseAsLock(string[] tokens)
        {
            switch (tokens[1].ToLower())
            {
                case "guns":
                case "mass":
                case "massdrivers":
                    return Order.Lock(tokens[2], entity.parts.Weapon.WeaponType.MASS_DRIVER);
                    break;
                case "energy":
                case "phasers":
                    return Order.Lock(tokens[2], entity.parts.Weapon.WeaponType.ENERGY);
                    break;
                case "torpedoes":
                case "torps":
                    return Order.Lock(tokens[2], entity.parts.Weapon.WeaponType.TORPEDO);
                    break;
                case "probes":
                    return Order.Lock(tokens[2], entity.parts.Weapon.WeaponType.PROBE);
                    break;
                default:
                    return Order.Lock(tokens[2], weaponName: tokens[1]);

            }


        }


        private static Order parseAsFire(string[] tokens)
        {
            switch (tokens[1].ToLower())
            {
                case "guns":
                case "mass":
                case "massdrivers":
                    return Order.Fire(entity.parts.Weapon.WeaponType.MASS_DRIVER);
                    break;
                case "energy":
                case "phasers":
                    return Order.Fire(entity.parts.Weapon.WeaponType.ENERGY);
                    break;
                case "torpedoes":
                case "torps":
                    return Order.Fire(entity.parts.Weapon.WeaponType.TORPEDO);
                    break;
                case "probes":
                    return Order.Fire(entity.parts.Weapon.WeaponType.PROBE);
                    break;
                default:
                    return Order.Fire(weaponName: tokens[1]);
                
            }
        }

        private static Order parseAsSetThrottle(string[] tokens)
        {
            float throttlePercent;
            if (float.TryParse(tokens[1], out throttlePercent))
                return Order.SetThrottle(throttlePercent);
            Console.WriteLine("Could not parse " + tokens[1] + " into a percent (0 - 100). :(");
            return Order.NullOrder();
        }

        private static Order parseAsSetCourse(string[] tokens)
        {
            float angleInDegrees;
            float throttle;
              
            if (float.TryParse(tokens[1], out angleInDegrees))
            {
                if (tokens.Length == 2)
                {
                    return Order.SetCourse(angleInDegrees);
                }
                if (float.TryParse(tokens[2], out throttle))
                {
                    return Order.SetCourse(angleInDegrees, throttle);
                }
                else
                {
                    return Order.SetCourse(angleInDegrees);
                }
            }
            Console.WriteLine("Could not parse " + tokens[1] + " into an angle. :(");
            return Order.NullOrder();
        }
    }
}
