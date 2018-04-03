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
                case "setcourse":
                    if ( tokens.Length > 1)
                        return parseAsSetCourse(tokens);
                    break;
                case "setthrottle":
                    if (tokens.Length > 1)
                        return parseAsSetThrottle(tokens);
                    break;
                case "lock":
                    if (tokens.Length > 1)
                        return Order.Lock(tokens[1]);
                    break;
                case "fire":
                    return Order.Fire();

            }
            return Order.NullOrder();
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
            if (float.TryParse(tokens[1], out angleInDegrees))
                return Order.SetCourse(angleInDegrees);// TODO: clamp angle to 0 - 360
            Console.WriteLine("Could not parse " + tokens[1] + " into an angle. :(");
            return Order.NullOrder();
        }
    }
}
