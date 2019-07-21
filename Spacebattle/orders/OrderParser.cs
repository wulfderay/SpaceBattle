using Spacebattle.entity;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public static Order ParseOrder(string plaintext, List<IDamageableEntity> entities)
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
                    if (tokens.Length >= 2)
                    {
                        return parseAsLock(tokens, entities);
                    }
                    break;
                case "load":
                    if (tokens.Length > 1)
                        return Order.Load(tokens[1]);
                    break;
                case "lower":
                case "drop":
                    return Order.LowerShields();
                case "raise":
                    return Order.RaiseShields();
                case "fire":
                    if (tokens.Length > 1)
                    {
                        return parseAsFire(tokens);
                    }
                    
                    return Order.Fire();
                case "scan":
                    if (tokens.Length > 1)
                    {
                        var scanTarget = entities.Where(entity => entity is Ship).Select(entity => entity as Ship).FirstOrDefault(ship => ship.Name.ToLower() == tokens[1].ToLower());
                        if ( scanTarget != null)
                            return Order.Scan(scanTarget);
                    }
                    break;
                case "status":
                    if (tokens.Length > 1)
                        return parseAsStatus(tokens, entities);
                    break;
                case "allstop":
                    return Order.AllStop();

            }
            return Order.NullOrder();
        }

        private static Order parseAsStatus(string[] tokens, List <IDamageableEntity> entities)
        {
            switch (tokens[1].ToLower())
            {
                case "power":
                    if (tokens.Length >= 3)
                        return Order.PowerStatus(entities.FirstOrDefault(x => x is Ship && x.Name.ToLower() == tokens[2].ToLower()) as Ship);
                    return Order.PowerStatus(null);
                default:
                    return Order.NullOrder();
            }
        }

        private static Order parseAsLock(string[] tokens, List<IDamageableEntity> entities)
        {

            var entityName = tokens.Length == 2 ? tokens[1] : tokens[2];
            var entityToLock = entities.FirstOrDefault(entity => entity.Name.ToLower() == entityName.ToLower());
            if (entityToLock == null)
                return Order.NullOrder();
            if (tokens.Length == 2)
                return Order.Lock(entityToLock);
            
            
            switch (tokens[1].ToLower())
            {
                case "guns":
                case "mass":
                case "massdrivers":
                    return Order.Lock(entityToLock, entity.parts.Weapon.WeaponType.MASSDRIVER);
                case "energy":
                case "phasers":
                    return Order.Lock(entityToLock, entity.parts.Weapon.WeaponType.PLASMABOLT);
                case "torpedoes":
                case "torps":
                    return Order.Lock(entityToLock, entity.parts.Weapon.WeaponType.TORPEDOTUBE);
                case "probes":
                    return Order.Lock(entityToLock, entity.parts.Weapon.WeaponType.PROBE);
                default:
                    return Order.Lock(entityToLock, weaponName: tokens[1]);

            }


        }


        private static Order parseAsFire(string[] tokens)
        {
            switch (tokens[1].ToLower())
            {
                case "guns":
                case "mass":
                case "massdrivers":
                    return Order.Fire(entity.parts.Weapon.WeaponType.MASSDRIVER);
                case "energy":
                case "phasers":
                    return Order.Fire(entity.parts.Weapon.WeaponType.PLASMABOLT);
                case "torpedoes":
                case "torps":
                    return Order.Fire(entity.parts.Weapon.WeaponType.TORPEDOTUBE);
                case "probes":
                    return Order.Fire(entity.parts.Weapon.WeaponType.PROBE);
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
