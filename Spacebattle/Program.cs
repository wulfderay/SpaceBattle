
using Spacebattle.entity;
using Spacebattle.entity.parts;
using System.Collections.Generic;
using System;
using Spacebattle.orders;
using Konsole;
using System.Linq;
using Konsole.Drawing;
using Spacebattle.Visualizer;
using Spacebattle.entity.parts.Weapon;
using Spacebattle.Entity.parts.Weapon;
using Spacebattle.Game;
using Spacebattle.Behaviours;
using System.Diagnostics;
using Spacebattle.Entity;
using Spacebattle.Orders;

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
            var ScanPanel = Window.Open(27, 1,95 ,9, "Scanner", LineThickNess.Single, ConsoleColor.Yellow, ConsoleColor.Black);
            var StatusPanel = Window.Open(27, 8, 95, 9, "Status", LineThickNess.Single, ConsoleColor.Yellow, ConsoleColor.Black);
            output = Window.Open(1, 16, 60, 24, "Output", LineThickNess.Double, ConsoleColor.White, ConsoleColor.Black);
            var input = new Window(1, 40, 60, 1, ConsoleColor.DarkGray, ConsoleColor.DarkMagenta);
            var shipList = Window.Open(62, 16, 60, 24, "Ships In Range", LineThickNess.Single, ConsoleColor.Cyan,
                ConsoleColor.Black);

            var sirWaffletonSnorkleBottom = LargeShip("SirWaffleton");
           

            var narwhal = new Ship(
                "SirBanannaSplit",
                new List<Reactor>() {Reactor.BigReactor(), Reactor.BigReactor(), Reactor.BigReactor()},
                new List<Shield>(),
                new List<IWeapon>() { new Lance("Lance", 50, 1, 10, 1200, 1000),
                    new Lance("Lance", 50, 1, 10, 1200, 1000) ,
                    new TorpedoTube("Torp", 100, 100, 20,(target) => spawnTorpedoFunc($"Torp{ConsoleVisualizer.GetEntitySymbol("SirBanannaSplit")}", target) ),
                    new TorpedoTube("Torp", 100, 100, 20,(target) => spawnTorpedoFunc($"Torp{ConsoleVisualizer.GetEntitySymbol("SirBanannaSplit")}", target) ),
                    new TorpedoTube("Torp", 100, 100, 20,(target) => spawnTorpedoFunc($"Torp{ConsoleVisualizer.GetEntitySymbol("SirBanannaSplit")}", target) )
                },
                new List<Engine>() {new Engine("MegaThruster", 50, 20, 10, 1000)},
                new List<CrewDeck>() { CrewDeck.Bridge()});

            var enterprise = new Ship(
                "Baldy",
                new List<Reactor>() { Reactor.BigReactor(), Reactor.SmallReactor() },
                bigShields(),
                new List<IWeapon>() { new Phaser("Phaser",100,10,0, 30, 1200) , new MassDriver("ScatterGun",100, 10, 0, 90, 20,200) },
                new List<Engine>() { new Engine("Engine",100,20, 50,100) },
                new List<CrewDeck>() { CrewDeck.MilitaryDeck(), CrewDeck.PleasureDeck()});


            var destroyer = new Ship(
                "Dragon",
                new List<Reactor>() { Reactor.BigReactor(), Reactor.BigReactor() },
                fastShields(),
                new List<IWeapon>() { new FireBreath("FireBreath", 100, 10, 0, 30, 200), new Phaser("Phaser", 100, 10, 0, 30, 200), new Lance("Lance", 100, 10, 0, 40, 1000)  },
                new List<Engine>() { new Engine("Engine", 100, 20, 50, 100) },
                new List<CrewDeck>() { CrewDeck.EngineeringDeck() , CrewDeck.Bridge()});

            var pooey = SmallShip("Pooey");

            var trymwing = new Ship(
                "Trymwing",
                new List<Reactor>() { Reactor.SmallReactor(), Reactor.SmallReactor(), Reactor.SmallReactor() },
                bigShields(),
                new List<IWeapon>() { new TorpedoTube("Torp",100, 100, 20, (target) => spawnTorpedoFunc($"Torp{ConsoleVisualizer.GetEntitySymbol("Trymwing")}", target)) , new Phaser("Phaser", 100, 10, 0, 20, 500) , new Lance("Lance", 100, 10, 0, 20, 1500) },
                new List<Engine>() { new Engine("Engine", 100, 20, 50, 100) },
                new List<CrewDeck>() { CrewDeck.EngineeringDeck(), CrewDeck.Bridge() });
            var carrier = new Ship(
                "McShipFace",
                new List<Reactor>() { Reactor.BigReactor(), Reactor.SmallReactor(), Reactor.SmallReactor() },
                bigShields(),
                new List<IWeapon>() { new TorpedoTube("Torp", 100, 100, 20,(target) => spawnTorpedoFunc($"Torp{ConsoleVisualizer.GetEntitySymbol("Carrier")}", target) )}, //new Hanger("Hanger", 300, 300, 100, MakeNewFighter) },
                new List<Engine>() { new Engine("Engine", 100, 20, 50, 100) },
                new List<CrewDeck>() { CrewDeck.EngineeringDeck(), CrewDeck.Bridge() });
            var cube = new Ship(
                "Cube",
                new List<Reactor>() {
                    Reactor.BigReactor(), Reactor.BigReactor(), Reactor.BigReactor(),
                    Reactor.BigReactor(), Reactor.BigReactor(), Reactor.BigReactor()
                },
                bigShields(12),
                new List<IWeapon>() { new Phaser("EnergyBeam", 50, 1, 10, 500, 500), new Phaser("EnergyBeam", 50, 1, 10, 500, 500) },
                new List<Engine>() { new Engine("Transwarp", 100, 100, 100, 1000) , new Engine("Transwarp", 100, 100, 100, 1000) },
                new List<CrewDeck>() {
                    CrewDeck.EngineeringDeck(5000),
                    CrewDeck.EngineeringDeck(5000),
                    CrewDeck.EngineeringDeck(5000),
                    CrewDeck.EngineeringDeck(5000),
                    CrewDeck.EngineeringDeck(5000),
                });
            var game = new GameEngine();
            game.FlavourTextEventHandler += OnFlavourText;
            game.ViewEventHandler += OnViewEvent;
           
          
            var yellowTeam = new List<Ship> {
               Destroyer("Destro")
            };
            var greenTeam = new List<Ship> {
                SmallShip("Zapper"),
                LargeShip("Triumph")
            };
            var blueTeam = new List<Ship>
            {
                cube
            };
            var team4 = new List<Ship>
            {
                MakeNewFighter(),
                MakeNewFighter(),
                MakeNewFighter(),
            };
            var team5 = new List<Ship>
            {
                MakeNewFighter(),

            };
            var team6 = new List<Ship>
            {
                MakeNewFighter(),

            };
            var team7 = new List<Ship>
            {
                MakeNewFighter(),

            };
            var team8 = new List<Ship>
            {
                MakeNewFighter(),

            };
            yellowTeam.ForEach(x => x.Team = 0);
            greenTeam.ForEach(x => x.Team = 1);
            blueTeam.ForEach(x => x.Team = 2);
            team4.ForEach(x => x.Team = 3);
            team5.ForEach(x => x.Team = 4);
            team6.ForEach(x => x.Team = 5);
            team7.ForEach(x => x.Team = 6);
            team8.ForEach(x => x.Team = 7);
            var startingEntities = new List<Ship>().
                Concat(yellowTeam).
                Concat(greenTeam).
               // Concat(blueTeam).
                //Concat(team4).
                //Concat(team5).
                //Concat(team6).
                //Concat(team7).
               // Concat(team8).
                ToList();
            flagship = yellowTeam[0];
            game.StartNewGame( flagship, startingEntities, 1000);

            UpdateDisplay(scannedShip, yellowTeam[0], game.Entities, ScanPanel,StatusPanel, radarPanel, shipList);

            while (!game.IsGameFinished())
            {
                input.Clear();
                input.WriteLine(ConsoleColor.Green,"Your orders, Sir?");
                Console.SetCursorPosition(20, 40);
                var order = OrderParser.ParseOrder(Console.ReadLine(), game.Entities.Where(entity => entity is IDamageableEntity).Select(entity => entity as IDamageableEntity).ToList());
                AlterViewForOrder(order);
                if (order.Type == Order.OrderType.SCAN || order.Type == Order.OrderType.STATUS) // scanning doesn't cost a turn.
                {
                    UpdateDisplay(scannedShip, yellowTeam[0], game.Entities, ScanPanel, StatusPanel, radarPanel, shipList);
                    continue;
                }
                OnFlavourText(game, new FlavourTextEventArgs() { name = "debug", level = FlavourTextEventArgs.LEVEL_DEBUG, message = $"Starting Round {game.CurrentRound}" , team = -1});
                game.RunOneRound(order);
                flagship = game.Flagship;
                UpdateDisplay(scannedShip, yellowTeam[0], game.Entities, ScanPanel,StatusPanel, radarPanel, shipList);
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

        private static Ship SmallShip(string name)
        {
            return new Ship(
                name,
                new List<Reactor>() { Reactor.SmallReactor(), Reactor.SmallReactor() },
                fastShields(2).Concat(fastShields(1)).ToList(),  // 2 layer shields.
                new List<IWeapon>() { PlasmaBolt.StandardPlasmaBolt(), PlasmaBolt.StandardPlasmaBolt() },
                new List<Engine>() { Engine.Thruster(), Engine.Thruster() },
                new List<CrewDeck>() { CrewDeck.Bridge(15) });
        }

        private static Ship Destroyer(string name)
        {
           return  new Ship(
                name,
                new List<Reactor>() { Reactor.SmallReactor(), Reactor.SmallReactor(), Reactor.SmallReactor() },
                fastShields(2).Concat(bigShields(1)).ToList(),
                new List<IWeapon>() {
                    new TorpedoTube("Torp", 100, 100, 20, (target) => spawnTorpedoFunc($"Torp{ConsoleVisualizer.GetEntitySymbol(name)}", target)),
                    new TorpedoTube("Torp", 100, 100, 20, (target) => spawnTorpedoFunc($"Torp{ConsoleVisualizer.GetEntitySymbol(name)}", target)),
                  //  new TorpedoTube("Torp", 100, 100, 20, (target) => spawnTorpedoFunc($"Torp{ConsoleVisualizer.GetEntitySymbol(name)}", target))
                },
                new List<Engine>() { Engine.CoreDrive()},
                new List<CrewDeck>() { CrewDeck.EngineeringDeck(35), CrewDeck.Bridge() });

        }

        private static Ship LargeShip(string name)
        {
            return new Ship(
                name,
                new List<Reactor> { Reactor.BigReactor(), Reactor.BigReactor() },
                bigShields(4),
                new List<IWeapon>() {
                    new Phaser("Phaser", 150, 1, 10, 200, 500) ,
                    new Phaser("Phaser", 150, 1, 10, 200, 500) ,
                    new Phaser("Phaser", 150, 1, 10, 200, 500) ,
                    new Lance("Lance", 150, 1, 30, 200, 1000) ,
                    new Lance("Lance", 150, 1, 30, 200, 1000) ,
                    new Lance("Lance", 150, 1, 30, 200, 1000) ,
                },
                new List<Engine>() { Engine.MainSail() },
                new List<CrewDeck>()
                {
                    CrewDeck.Bridge(),
                    CrewDeck.EngineeringDeck(),
                    CrewDeck.MilitaryDeck(10),
                }

                );
        }

            private static Ship MakeNewFighter()
        {
            var fighter = new Ship(
                "Fighter"+GameEngine.Random(0,20),
                new List<Reactor>() { Reactor.SmallReactor(), Reactor.SmallReactor() },
                bigShields(2),
                new List<IWeapon>() { PlasmaBolt.StandardPlasmaBolt()},
                new List<Engine>() { new Engine("Engine", 100, 20, 50, 100) },
                new List<CrewDeck>() { CrewDeck.Bridge(3) });
            fighter.AddBehaviour(new BasicAiBehaviour(fighter));
            return fighter;
        }

        private static void OnViewEvent(object sender, ViewEventArgs e)
        {
            scannedShip = ((ScanEvent)e).Ship;
            

        }

        private static Torpedo spawnTorpedoFunc(string name, IDamageableEntity target)
        {
            return new Torpedo(name, target, 15, 300, 120, 400);
        }

        private static List<Shield> fastShields(int num = 6)
        {
            var result = new List<Shield>();
            for (int i = 0; i < num; i++)
            {
                result.Add(Shield.FastRegenshield("Shield " + (i + 1), 360/num, 360/num * i));
            }
            return result;
        }

        private static List<Shield> bigShields(int num = 6)
        {
            var result = new List<Shield>();
            for (int i = 0; i < num; i++)
            {
                result.Add(Shield.Bigshield("Shield " + (i + 1), 360/num, 360/num * i));
            }
            return result;
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
