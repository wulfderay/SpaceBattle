
using Spacebattle.entity;
using Spacebattle.entity.parts;
using System.Collections.Generic;
using System;
using System.Drawing;
using Spacebattle.orders;
using Konsole;

namespace Spacebattle
{
    class Program
    {
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
             *  TODO: guns should shoot at an angle.... and the sheilds should provide protection so long as they are in the way.
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
             * for example, sheilds won't charge if there isn't enough power. For now, that's the only problem. Needs more fleshing out though.
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


            var con = new Window(200, 50);
            con.WriteLine("starting client server demo");
            var visualizer = new Window(1, 4, 20, 20, ConsoleColor.Gray, ConsoleColor.DarkBlue, con);
            var output = new Window(25, 4, 20, 20, con);
            visualizer.WriteLine("Visualizer");
            visualizer.WriteLine("------");
            output.WriteLine("Output");
            output.WriteLine("------");
            visualizer.WriteLine("<-- PUT some long text to show wrapping");
            output.WriteLine(ConsoleColor.DarkYellow, "--> PUT some long text to show wrapping");
            output.WriteLine(ConsoleColor.Red, "<-- 404|Not Found|some long text to show wrapping|");
            visualizer.WriteLine(ConsoleColor.Red, "--> 404|Not Found|some long text to show wrapping|");




            var shaunShip = new Ship(
                "ShaunShip",
                new List<Reactor>() {Reactor.SmallReactor()},
                new List<Sheild>() {Sheild.FastRegenSheild()},
                new List<Weapon>() { new Weapon("Lance", 50, 1, 10, 100, 500)},
                new List<Engine>() {new Engine("MegaThruster", 50, 20, 100, 1000)},
                new List<CrewDeck>() { CrewDeck.Bridge() });

            var shipOne = new Ship(
                "Enterprise",
                new List<Reactor>() { Reactor.BigReactor(), Reactor.SmallReactor() },
                new List<Sheild>() { Sheild.BigSheild(), Sheild.FastRegenSheild()},
                new List<Weapon>() { new Weapon("Gun",100,10,0, 30, 200) , new Weapon("Gun",100, 10, 0, 30, 200) },
                new List<Engine>() { new Engine("Engine",100,20, 50,100) },
                new List<CrewDeck>() { CrewDeck.MilitaryDeck(), CrewDeck.PleasureDeck()});


            var shipTwo = new Ship(
                "destroyer",
                new List<Reactor>() { Reactor.SmallReactor(), Reactor.SmallReactor() },
                new List<Sheild>() {  Sheild.FastRegenSheild() },
                new List<Weapon>() { new Weapon("Gun", 100, 10, 0, 30, 200) },
                new List<Engine>() { new Engine("Engine", 100, 20, 50, 100) },
                new List<CrewDeck>() { CrewDeck.EngineeringDeck() , CrewDeck.Bridge()});

            var pooey = new Ship(
                "pooey",
                new List<Reactor>() { Reactor.SmallReactor(), Reactor.SmallReactor() },
                new List<Sheild>() {  Sheild.FastRegenSheild() },
                new List<Weapon>() { new Weapon("Gun", 100, 10, 0, 30, 200) },
                new List<Engine>() { new Engine("Engine", 100, 20, 50, 100) },
                new List<CrewDeck>() { CrewDeck.EngineeringDeck(), CrewDeck.Bridge() });

            var game = new GameEngine();
            game.FlavourTextEventHandler += OnFlavourText;
            game.StartNewGame(new List<Ship> { shaunShip }, new List<Ship> {  pooey }, 1000);

            



            while (!game.IsGameFinished())
            {
                // TODO: Get order from console 
                Console.WriteLine("Your orders, Sir?");
                var order = OrderParser.ParseOrder(Console.ReadLine());

                game.RunOneRound(order);

            }
            if (game.GetWhichTeamWon() == -1)
            {
                Console.WriteLine("It was a tie!");
            }
            else 
            {
               
                Console.WriteLine("Team "+ game.GetWhichTeamWon() + " Won!!");


            }
        }

        private static void OnFlavourText(object sender, FlavourTextEventArgs e)
        {
            Console.WriteLine("["+e.name+"]: "+e.message);
        }
    }
}
