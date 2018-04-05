
using Spacebattle.entity;
using Spacebattle.entity.parts;
using System.Collections.Generic;
using System;
using Spacebattle.orders;
using Konsole;
using System.Linq;
using Konsole.Drawing;
using Spacebattle.Visualizer;

namespace Spacebattle
{
    class Program
    {
        private static Window output;

        static void Main(string[] args)
        {
            /*
             * This is the engine for a begin style space battle. You write commands (or on phone pick actions) and each turn those commands (and those of the bad guys) are executed.
             * 
             *  parts:
             *  
             *  reactors provide power
             *  shields provide protection
             *  guns shoot
             *  engines move
             *  crewDecks provide repairs ( i guess crew would be the hit points here?)
             *  
             *  TODO:
             *  Sensors scan
             *  Transporters Beam
             *  
             *  
             *  
             *  TODO: guns should shoot at an angle.... and the shields should provide protection so long as they are in the way.
             *  Commands to implement:
             *  Report
             *  fire
             *  helm
             *  load
             *  unload
             *  destruct
             *  lock
             * 
             * TODo: How should parts consume power?
             * I guess there are two ways.... an upkeep step and a per use charge..
             * for example, shields need some power just to keep running... but they need lots of power to regenerate
             * and crew decks need some power to keep the lights and life support on.. but they need more to do repairs
             * and engines would use energy proportional to their throttle... but not much when idle
             * guns would probably not use power unless shooting.
             * reactors provide power, of course.... if we anted to model them after fusion reactors, we could make them require some power as well, but that's going to far. :>)
             * 
             * Right now we have an upkeep power requirement... if you consume too much power, you don't have enough energy to do per-use things
             * for example, shields won't charge if there isn't enough power. For now, that's the only problem. Needs more fleshing out though.
             * 
             * Weapons need more work. Accuracy on some weapons should fall off with distance, damage with distance for others.
             * Projectile weapons also need to be implemented. Also, the concept of loading.
             * Also, firing with a specific weapon.
             * 
             * TODO: Status effects on ships and shipparts
             * for example, reimplement destroyed as a status, and create derelict status for dead but functional ships.
             * 
             * Idea: what about a "repulser drive" aka antigravity... it would counteract the effects of gravits so that gravity scanners wouldn't detect a ship.... but it would
             * need to disable the engines... otherwise you could travel light speed immididiately...
             * 
             * 
             */
            setConsoleSize();
            var DebugPanel = Window.Open(1, 41, 100, 10, "Debug", LineThickNess.Single, ConsoleColor.Red, ConsoleColor.Black);
            ConsoleVisualizer.DebugEventHandler += (sender, eventArgs) =>
                DebugPanel.WriteLine("[" + eventArgs.From + "] " + eventArgs.Message);
            var radarPanel = Window.Open(1, 1, 25, 14, "Radar", LineThickNess.Single, ConsoleColor.Green,
                ConsoleColor.Black);
            radarPanel.WriteLine("Radar");
            var ScanPanel = new Window(27, 1,100 ,14, ConsoleColor.Yellow, ConsoleColor.DarkGray);
            output = new Window(1, 16, 60, 24,  ConsoleColor.White, ConsoleColor.DarkCyan);
            var input = new Window(1, 40, 60, 1, ConsoleColor.DarkGray, ConsoleColor.DarkMagenta);
            var shipList = Window.Open(62, 16, 60, 24, "Ships In Range", LineThickNess.Single, ConsoleColor.Cyan,
                ConsoleColor.Black);

            var shaunShip = new Ship(
                "Lance",
                new List<Reactor>() {Reactor.BigReactor()},
                new List<Shield>() {Shield.FastRegenshield()},
                new List<Weapon>() { new Weapon("Lance", 50, 1, 10, 100, 500)},
                new List<Engine>() {new Engine("MegaThruster", 50, 20, 100, 1000)},
                new List<CrewDeck>() { CrewDeck.Bridge()});

            var shipOne = new Ship(
                "Enterprise",
                new List<Reactor>() { Reactor.BigReactor(), Reactor.SmallReactor() },
                new List<Shield>() { Shield.Bigshield(), Shield.FastRegenshield()},
                new List<Weapon>() { new Weapon("Phaser",100,10,0, 30, 1200) , new Weapon("ScatterGun",100, 10, 0, 100, 200) },
                new List<Engine>() { new Engine("Engine",100,20, 50,100) },
                new List<CrewDeck>() { CrewDeck.MilitaryDeck(), CrewDeck.PleasureDeck()});


            var shipTwo = new Ship(
                "destroyer",
                new List<Reactor>() { Reactor.SmallReactor(), Reactor.SmallReactor() },
                new List<Shield>() {  Shield.FastRegenshield() },
                new List<Weapon>() { new Weapon("Gun", 100, 10, 0, 30, 200), new Weapon("Gun", 100, 10, 0, 30, 200) },
                new List<Engine>() { new Engine("Engine", 100, 20, 50, 100) },
                new List<CrewDeck>() { CrewDeck.EngineeringDeck() , CrewDeck.Bridge()});

            var pooey = new Ship(
                "Pooey",
                new List<Reactor>() { Reactor.SmallReactor(), Reactor.SmallReactor() },
                new List<Shield>() {  Shield.FastRegenshield() },
                new List<Weapon>() { new Weapon("Gun", 100, 10, 0, 30, 200) },
                new List<Engine>() { new Engine("Engine", 100, 20, 50, 100) },
                new List<CrewDeck>() { CrewDeck.EngineeringDeck(), CrewDeck.Bridge() });

            var game = new GameEngine();
            game.FlavourTextEventHandler += OnFlavourText;
            var redteam = new List<Ship> {
                shipOne,
                shaunShip,
                new Ship(
                "Cube",
                new List<Reactor>() { Reactor.BigReactor(), Reactor.BigReactor(), Reactor.BigReactor() },
                new List<Shield>() {  },
                new List<Weapon>() { new Weapon("PlasmaBolt+", 100, 10, 0, 300, 400) },
                new List<Engine>() { new Engine("Transwarp", 100, 20, 50, 1000) },
                new List<CrewDeck>() { CrewDeck.EngineeringDeck(),CrewDeck.EngineeringDeck(),CrewDeck.EngineeringDeck() })
        };
            var blueteam = new List<Ship> {
                pooey,
                shipTwo,
                new Ship(
                "Vega",
                new List<Reactor>() { Reactor.SmallReactor(), Reactor.SmallReactor() },
                new List<Shield>() {  Shield.FastRegenshield(), Shield.FastRegenshield()},
                new List<Weapon>() { new Weapon("PlasmaBolt", 100, 10, 20, 60, 500) },
                new List<Engine>() { new Engine("Small Engine", 100, 20, 50, 20), new Engine("Hyper Drive", 30, 120, 50, 300)  },
                new List<CrewDeck>() { new CrewDeck("Bridge", 50, 20, 10, 15, .1f) }),
                new Ship(
                "Vega2",
                new List<Reactor>() { Reactor.SmallReactor(), Reactor.SmallReactor() },
                new List<Shield>() {  Shield.FastRegenshield(), Shield.FastRegenshield()},
                new List<Weapon>() { new Weapon("PlasmaBolt", 100, 10, 20, 60, 500) },
                new List<Engine>() { new Engine("Small Engine", 100, 20, 50, 20), new Engine("Hyper Drive", 30, 120, 50, 300)  },
                new List<CrewDeck>() { new CrewDeck("Bridge", 50, 20, 10, 15, .1f) })
        };
            var bothTeams = new List<Ship>();
            bothTeams.AddRange(redteam);
            bothTeams.AddRange(blueteam);
            game.StartNewGame(redteam, blueteam, 1000);

            UpdateDisplay(redteam[0].LockedShip, redteam[0], bothTeams, ScanPanel, radarPanel, shipList);

            
            while (!game.IsGameFinished())
            {
                // TODO: Get order from console 
                input.WriteLine(ConsoleColor.Green,"Your orders, Sir?");
                Console.SetCursorPosition(20, 40);
                var order = OrderParser.ParseOrder(Console.ReadLine());

                game.RunOneRound(order);
                UpdateDisplay(redteam[0].LockedShip, redteam[0], bothTeams, ScanPanel, radarPanel, shipList);


            }
            if (game.GetWhichTeamWon() == -1)
            {
                output.WriteLine("It was a tie!");
            }
            else 
            {
                output.WriteLine("Team "+ game.GetWhichTeamWon() + " Won!!");
            }
            
           while (Console.ReadKey().Key != ConsoleKey.Escape)
            {

            }
        }

        private static void OnFlavourText(object sender, FlavourTextEventArgs e)
        {
            if ( e.team == -1)
                output.WriteLine("["+e.name+"]: "+e.message);
            if (e.team == 0)
                output.WriteLine(ConsoleColor.Blue, "[" + e.name + "]: " + e.message);
            if (e.team == 1)
                output.WriteLine(ConsoleColor.Red, "[" + e.name + "]: " + e.message);
        }

        static void setConsoleSize()
        {
            Console.SetWindowPosition(0, 0);   // sets window position to upper left
            Console.SetBufferSize(122, 54);   // make sure buffer is bigger than window
            Console.SetWindowSize(122, 54);   //set window size to almost full screen 
        }  // End  setConsoleSize()

        public static void PrintShip(Ship ship, IConsole window , ConsoleColor color)
        {
            window.WriteLine(color, "Name:" + ship.GetName() +
                " Crew:" + ship.CrewDecks.Select(x => (int)x.GetCrew()).Sum() +
                " Mass: " + ship.Mass +
                " Power:" + ship.Power);
            window.WriteLine(color, string.Join(" ", ship.Reactors.Select(x => x.ToString())));
            window.WriteLine(color, string.Join(" ", ship.Shields.Select(x => x.ToString())));
            window.WriteLine(color, string.Join(" ", ship.Guns.Select(x => x.ToString())));
            window.WriteLine(color, string.Join(" ", ship.Engines.Select(x => x.ToString())));
            window.WriteLine(color, string.Join(" ", ship.CrewDecks.Select(x => x.ToString())));
            if (ship.IsDestroyed())
                window.WriteLine("(Destroyed)");
        }

        public static void UpdateDisplay(Ship target, Ship flagship, List<Ship> ships, IConsole scanPanel, IConsole radarPanel, IConsole shipListPanel)
        {
            if (target != null)
            {
                PrintShip(target, scanPanel, ConsoleColor.Black);
                scanPanel.WriteLine("");
            }
            PrintShip(flagship, scanPanel, ConsoleColor.White);
            ConsoleVisualizer.DrawRadar(radarPanel, ships, flagship, 1000);
            ConsoleVisualizer.DrawShipList(shipListPanel, ships, flagship);
        }
    }
}
