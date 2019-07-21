
using Spacebattle.entity;
using System.Collections.Generic;
using System;
using Spacebattle.orders;
using Konsole;
using System.Linq;
using Konsole.Drawing;
using Spacebattle.Visualizer;
using Spacebattle.Game;
using Spacebattle.Orders;
using Spacebattle.Configuration.EntityConstructors;
using Newtonsoft.Json;
using System.IO;
using Spacebattle.Configuration.Schema;

namespace Spacebattle
{
    class Program
    {
        private static IConsole output;

        public static Ship scannedShip = null;

        public static Ship flagship { get; private set; }

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
             *  Commands to implement:
             *  Report
             *  fire
             *  helm
             *  load
             *  unload
             *  destruct
             *  lock
             *  status
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

            var radarPanel = Window.Open(1, 1, 25, 15, "Radar", LineThickNess.Single, ConsoleColor.Green,
                ConsoleColor.Black);
            radarPanel.WriteLine("Radar");
            var ScanPanel = Window.Open(27, 1, 95, 9, "Scanner", LineThickNess.Single, ConsoleColor.Yellow, ConsoleColor.Black);
            var StatusPanel = Window.Open(27, 8, 95, 9, "Status", LineThickNess.Single, ConsoleColor.Yellow, ConsoleColor.Black);
            output = Window.Open(1, 16, 60, 24, "Output", LineThickNess.Double, ConsoleColor.White, ConsoleColor.Black);
            var input = new Window(1, 40, 60, 1, ConsoleColor.DarkGray, ConsoleColor.DarkMagenta);
            var shipList = Window.Open(62, 16, 60, 24, "Ships In Range", LineThickNess.Single, ConsoleColor.Cyan,
                ConsoleColor.Black);

            var game = new GameEngine();
            game.FlavourTextEventHandler += OnFlavourText;
            game.ViewEventHandler += OnViewEvent;

            
            var startingEntities = new List<Ship>().
                Concat(ReadTeamFromFile(1)).
                Concat(ReadTeamFromFile(2)).
                Concat(ReadTeamFromFile(3)).
                Concat(ReadTeamFromFile(4)).
                Concat(ReadTeamFromFile(5)).
                Concat(ReadTeamFromFile(6)).
                Concat(ReadTeamFromFile(7)).
                Concat(ReadTeamFromFile(8)).
                ToList();
            flagship = startingEntities.First(X => X.Team == 0);
            game.StartNewGame( flagship, startingEntities, 1000);

            UpdateDisplay(scannedShip, flagship, game.Entities, ScanPanel,StatusPanel, radarPanel, shipList);

            while (!game.IsGameFinished())
            {
                input.Clear();
                input.WriteLine(ConsoleColor.Green,"Your orders, Sir?");
                Console.SetCursorPosition(20, 40);
                var order = OrderParser.ParseOrder(Console.ReadLine(), game.Entities.Where(entity => entity is IDamageableEntity).Select(entity => entity as IDamageableEntity).ToList());
                AlterViewForOrder(order);
                if (order.Type == Order.OrderType.SCAN || order.Type == Order.OrderType.STATUS) // scanning doesn't cost a turn.
                {
                    UpdateDisplay(scannedShip, flagship, game.Entities, ScanPanel, StatusPanel, radarPanel, shipList);
                    continue;
                }
                OnFlavourText(game, new FlavourTextEventArgs() { name = "debug", level = FlavourTextEventArgs.LEVEL_DEBUG, message = $"Starting Round {game.CurrentRound}" , team = -1});
                game.RunOneRound(order);
                flagship = game.Flagship;
                UpdateDisplay(scannedShip, flagship, game.Entities, ScanPanel,StatusPanel, radarPanel, shipList);
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

        private static IEnumerable<Ship> ReadTeamFromFile(int TeamNumber)
        {
            var fileName = $"Team{TeamNumber}.json";
            if (!File.Exists(fileName))
            {
                return new List<Ship>();
            }
            var teamJson = File.ReadAllText($"Team{TeamNumber}.json");
            var schema = JsonConvert.DeserializeObject<List<ShipSchema>>(teamJson); // todo: make it possible to create a ship from a file.
            var ships = schema.Select(ship => ShipFactory.Construct(ship)).ToList();
            ships.ForEach(x => x.Team = TeamNumber - 1);
            return ships;
        }

        private static void AlterViewForOrder(Order order)
        {
            switch (order.Type)
            {
                case Order.OrderType.HELM:
                    OnFlavourText(flagship, new FlavourTextEventArgs { name = flagship.Name, message = "Setting course for heading " + (order as HelmOrder).AngleInDegrees, team = flagship.Team });
                    break;

                case Order.OrderType.SCAN:
                    OnViewEvent(flagship, ViewEventArgs.Scan((order as ScanOrder).ShipToScan));
                    break;
                case Order.OrderType.STATUS:
                    PrintStatus(order as StatusOrder);
                    break;
                case Order.OrderType.NULL_ORDER:
                    OnFlavourText(flagship, new FlavourTextEventArgs { name = flagship.Name, message = "Sorry, what was that, Captain?", team = flagship.Team });
                    break;

            }
        }

        private static void PrintStatus(StatusOrder order)
        {
            var ship = order.Ship != null ? order.Ship : flagship;
            switch (order.StatusType)
            {
                case StatusType.POWER:
                    OnFlavourText( $"Power Status ({ship.Name}):" );
                    OnFlavourText($" Reactors +{ship.Reactors.Sum(x => x.Produce())}eu");
                    OnFlavourText($" Engines -{ship.Engines.Sum(x => x.GetUpkeepCost())}eu");
                    OnFlavourText($" Shields -{ship.Shields.Sum(x => x.GetUpkeepCost())}eu");
                    OnFlavourText($" Weapons -{ship.Weapons.Sum(x => x.GetUpkeepCost())}eu");
                    OnFlavourText($" CrewDecks -{ship.CrewDecks.Sum(x => x.GetUpkeepCost())}eu");
                    OnFlavourText($" Total leftover power: {ship.Power}eu");
                    break;
                default:

                    break;
            }
        }

        private static void OnFlavourText(string message)
        {
            OnFlavourText(flagship, new FlavourTextEventArgs { name = flagship.Name, message = message, team = flagship.Team });
        }

        private static void OnViewEvent(object sender, ViewEventArgs e)
        {
            scannedShip = ((ScanEvent)e).Ship;
            

        }
       

        private static void OnFlavourText(object sender, FlavourTextEventArgs e)
        {
            if ( e.name.Contains(flagship.Name) || e.level <= FlavourTextEventArgs.LEVEL_INFO)
                output.WriteLine(ConsoleVisualizer.GetTeamColor(e.team), "[" + e.name + "]: " + e.message);
        }

        static void setConsoleSize()
        {
            Console.SetWindowPosition(0, 0);   // sets window position to upper left
            Console.SetBufferSize(122, 100);   // make sure buffer is bigger than window
            Console.SetWindowSize(122, 44);   //set window size to almost full screen 
        }  // End  setConsoleSize()


        public static void UpdateDisplay(IDamageableEntity target, IDamageableEntity flagship, List<IGameEntity> ships, IConsole scanPanel,  IConsole statusPanel, IConsole radarPanel, IConsole shipListPanel)
        {
            if (target != null)
            {
                ConsoleVisualizer.PrintShip((Ship)target, scanPanel, ConsoleColor.White);
            }

            ConsoleVisualizer.PrintShip((Ship)flagship,statusPanel , ConsoleColor.White);
            ConsoleVisualizer.DrawRadar(radarPanel, ships, flagship, 2000);
            ConsoleVisualizer.DrawShipList(shipListPanel, ships, flagship);
        }
    }
}
