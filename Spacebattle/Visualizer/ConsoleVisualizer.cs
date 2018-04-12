using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Konsole;
using Spacebattle.entity;

namespace Spacebattle.Visualizer
{
    // should we make this an interface(so that we can just plug in a different visualizer)? dunno.
    public class ConsoleVisualizer 
    {
        

        static int[] cColors = { 0x000000, 0x000080, 0x008000, 0x008080, 0x800000, 0x800080, 0x808000, 0xC0C0C0, 0x808080, 0x0000FF, 0x00FF00, 0x00FFFF, 0xFF0000, 0xFF00FF, 0xFFFF00, 0xFFFFFF };

        public static event EventHandler<DebugEventArgs> DebugEventHandler;
        /// <summary>
        /// Taken from the excellent answer on stack overflow: https://stackoverflow.com/questions/33538527/display-a-image-in-a-console-application
        /// </summary>
        /// <param name="cValue"></param>
        public static void ConsoleWritePixel(Color cValue)
        {
            Color[] cTable = cColors.Select(x => Color.FromArgb(x)).ToArray();
            char[] rList = { (char)9617, (char)9618, (char)9619, (char)9608 }; // 1/4, 2/4, 3/4, 4/4
            int[] bestHit = { 0, 0, 4, int.MaxValue }; //ForeColor, BackColor, Symbol, Score

            for (int rChar = rList.Length; rChar > 0; rChar--)
            {
                for (int cFore = 0; cFore < cTable.Length; cFore++)
                {
                    for (int cBack = 0; cBack < cTable.Length; cBack++)
                    {
                        int R = (cTable[cFore].R * rChar + cTable[cBack].R * (rList.Length - rChar)) / rList.Length;
                        int G = (cTable[cFore].G * rChar + cTable[cBack].G * (rList.Length - rChar)) / rList.Length;
                        int B = (cTable[cFore].B * rChar + cTable[cBack].B * (rList.Length - rChar)) / rList.Length;
                        int iScore = (cValue.R - R) * (cValue.R - R) + (cValue.G - G) * (cValue.G - G) + (cValue.B - B) * (cValue.B - B);
                        if (!(rChar > 1 && rChar < 4 && iScore > 50000)) // rule out too weird combinations
                        {
                            if (iScore < bestHit[3])
                            {
                                bestHit[3] = iScore; //Score
                                bestHit[0] = cFore;  //ForeColor
                                bestHit[1] = cBack;  //BackColor
                                bestHit[2] = rChar;  //Symbol
                            }
                        }
                    }
                }
            }
            Console.ForegroundColor = (ConsoleColor)bestHit[0];
            Console.BackgroundColor = (ConsoleColor)bestHit[1];
            Console.Write(rList[bestHit[2] - 1]);
        }

        /*
         * Bitmap bmpSrc = new Bitmap(@"C:\Users\mlee\Documents\GitHub\SpaceBattle\HuwnC.gif", true);
            ConsoleVisualizer.ConsoleVisualizer.WriteImage(bmpSrc);
         */
        /// <summary>
        /// Taken from the excellent answer on stack overflow: https://stackoverflow.com/questions/33538527/display-a-image-in-a-console-application
        /// </summary>
        /// <param name="source"></param>
        public static void WriteImage(Bitmap source)
        {
            int sMax = 39;
            decimal percent = Math.Min(decimal.Divide(sMax, source.Width), decimal.Divide(sMax, source.Height));
            Size dSize = new Size((int)(source.Width * percent), (int)(source.Height * percent));
            Bitmap bmpMax = new Bitmap(source, dSize.Width * 2, dSize.Height);
            for (int i = 0; i < dSize.Height; i++)
            {
                for (int j = 0; j < dSize.Width; j++)
                {
                    ConsoleWritePixel(bmpMax.GetPixel(j * 2, i));
                    ConsoleWritePixel(bmpMax.GetPixel(j * 2 + 1, i));
                }
                Console.WriteLine();
            }
            Console.ResetColor();
        }


        public static void DrawTextXY(int x, int y, string message)
        {
            var oldX = Console.CursorLeft;
            var oldY = Console.CursorTop;
            Console.SetCursorPosition(x, y);
            Console.Write(message);
            Console.SetCursorPosition(oldX, oldY);
        }

        public static void  DrawRadar(IConsole window, List<Ship> ships, IEntity centreEntity, float range)
        {
            window.Clear();
            var scaleX = window.WindowWidth / range;
            var scaleY = window.WindowHeight / range;
            var centreX = window.WindowWidth / 2;
            var centreY = window.WindowHeight / 2;
            foreach (var ship in ships)
            {
                if (ship.DistanceTo(centreEntity) > range)
                    continue;
                var distanceFromCenter = ship.Position - centreEntity.Position;
                var x = (int)(distanceFromCenter.X * scaleX) + centreX;
                var y = (int)(distanceFromCenter.Y * scaleY) + centreY;
                if (x < 0 || y < 0)
                {
                    OnDebug("Radar", "Didn't draw " + ship.GetName() + " at " + x + " " + y);
                    continue;
                }

                if (ship.IsDestroyed())
                {
                    window.ForegroundColor = ConsoleColor.Red;
                }
                else
                {
                    window.ForegroundColor = GetShipColor(ship);
                }
                window.PrintAt(x,y, GetShipSymbol(ship));
                OnDebug("Radar", "Drew "+ship.GetName()+" at "+x+" "+y);

            }
        }

        public static void DrawShipList(IConsole window, List<Ship> ships, IEntity centreEntity)
        {
            window.Clear();
            window.WriteLine(ConsoleColor.Yellow,"Name".PadLeft(12) + "\t" + "Range" + "\t" + "Bearing");
            foreach (var ship in ships.OrderBy(x => x.DistanceTo(centreEntity)))
            {
                var shipColor = ship.IsDestroyed() ? ConsoleColor.Red : GetShipColor(ship);
                window.Write(shipColor, GetShipSymbol(ship) + " ");
                window.WriteLine(
                    ship.GetName().PadLeft(10) + "\t" + 
                    (int)ship.DistanceTo(centreEntity) + "\t" + 
                    (int)centreEntity.DirectionInDegreesTo(ship) + '\t'+
                    (int)ship.Position.X+ "\t"+
                    (int)ship.Position.Y);
            }
        }

        public static char GetShipSymbol(Ship ship)
        {
            List<char> symbols = new List<char> { '@', '#', '$', '%', '&', '*', 'π', 'Σ', 'Φ', 'φ', 'α', 'ß', 'δ', '■', 'Ω', '¥', 'Θ', '≡', '±', };
            var index = (Math.Abs(ship.GetName().GetHashCode()) % symbols.Count);
            return symbols[index];
        }

        public static ConsoleColor GetShipColor(Ship ship)
        {
            List<ConsoleColor> colors = new List<ConsoleColor> { ConsoleColor.Yellow, ConsoleColor.Green, ConsoleColor.Blue, ConsoleColor.DarkGray, ConsoleColor.DarkYellow, ConsoleColor.White , ConsoleColor.DarkMagenta};
            var index = (Math.Abs(ship.GetName().GetHashCode()) % colors.Count);
            return colors[index];
        }

        protected static void OnDebug(string from, string message)
        {
            DebugEventHandler?.Invoke(null, new DebugEventArgs() { From = from, Message = message });
        }

        public static void PrintShip(Ship ship, IConsole window, ConsoleColor color)
        {
            window.WriteLine(color, "Name:" + ship.GetName() +
                " Crew:" + ship.CrewDecks.Select(x => (int)x.GetCrew()).Sum() +
                " Mass: " + ship.Mass +
                " Power:" + ship.Power);
            PrintReactors(ship, window, color);
            PrintShields(ship, window, color);
            PrintWeapons(ship, window, color);
            PrintEngines(ship, window, color);
            PrintCrewDecks(ship, window, color);
            if (ship.IsDestroyed())
                window.WriteLine("(Destroyed)");
        }

        private static void PrintWeapons(Ship ship, IConsole window, ConsoleColor color)
        {
            window.Write(color, "Weapons: |");
            foreach (var weapon in ship.Weapons)
            {
                window.Write(color, weapon.GetName()+" H:");
                window.Write(ColorFromTuple(weapon.Health), HealthBarFromTuple(weapon.Health));
                window.Write(color, " S:-");
               // window.Write(ColorFromTuple(weapon.Strength), HealthBarFromTuple(weapon.Strength));
                window.Write(color, "|");
            }
            window.WriteLine(color, string.Empty);
        }

        private static void PrintEngines(Ship ship, IConsole window, ConsoleColor color)
        {
            window.Write(color, "Engines: |");
            foreach (var engine in ship.Engines)
            {
                window.Write(color, " H:");
                window.Write(ColorFromTuple(engine.Health), HealthBarFromTuple(engine.Health));
                window.Write(color, "|");
            }
            window.WriteLine(color, string.Empty);
        }

        private static void PrintCrewDecks(Ship ship, IConsole window, ConsoleColor color)
        {
            window.Write(color, "Crew Decks: |");
            foreach (var crewDeck in ship.CrewDecks)
            {
                window.Write(color, crewDeck.GetName() + " H:");
                window.Write(ColorFromTuple(crewDeck.Health), HealthBarFromTuple(crewDeck.Health));
                window.Write(color, " C:" + crewDeck.GetCrew() + "|");
            }
            window.WriteLine(color, string.Empty);
        }

        private static void PrintShields(Ship ship, IConsole window, ConsoleColor color)
        {
            window.Write(color, "Shields: |");
            foreach (var shield in ship.Shields)
            {
                window.Write(color, " H:");
                window.Write(ColorFromTuple(shield.Health), HealthBarFromTuple(shield.Health));
                window.Write(color, " S:");
                window.Write(ColorFromTuple(shield.Strength), HealthBarFromTuple(shield.Strength) );
                window.Write(color, "|");
            }
            window.WriteLine(color, string.Empty);
        }

        private static void PrintReactors(Ship ship, IConsole window, ConsoleColor color)
        {
            window.Write(color, "Reactors: |");
            foreach (var reactor in ship.Reactors)
            {
                window.Write(color, " H:");
                window.Write(ColorFromTuple(reactor.Health), HealthBarFromTuple(reactor.Health));
                window.Write(color, " P:" + reactor.Produce() + "|");
            }
            window.WriteLine(color, string.Empty);
        }
        private static void PrintColorBand(IConsole window)
        {

            for (var i = 0; i < 100; i+=10)
            {
                window.Write(ColorFromTuple(new Tuple<float, float>(i,100)), HealthBarFromTuple(new Tuple<float, float>(i, 100)));
            }
            window.WriteLine( string.Empty);
        }
        private static string HealthBarFromTuple(Tuple<float, float> health)
        {
            var ratio = (health.Item1 / health.Item2) * 100;
            if (ratio > 60)
                return "▓";

            if (ratio > 30)
                return "▒";

             if (ratio >1)
                return "░";
            return "X";
        }

        private static ConsoleColor ColorFromTuple(Tuple<float, float> health)
        {
            var ratio = (health.Item1 / health.Item2) * 100;
            if (ratio > 80)
                return ConsoleColor.Green;

            if (ratio > 64)
                return ConsoleColor.DarkGreen;
            if (ratio > 48)
                return ConsoleColor.Yellow;
            if (ratio > 32)
                return ConsoleColor.DarkYellow;
            if (ratio > 16)
                return ConsoleColor.Red;

            return ConsoleColor.DarkRed;
        }
    }
}