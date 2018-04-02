using Spacebattle.entity;
using Spacebattle.orders;
using Spacebattle.physics;
using System;
using System.Collections.Generic;
using System.Linq;
using static Spacebattle.orders.Order;

namespace Spacebattle
{
    public class GameEngine:IFlavourTextProvider
    {

        private List<Ship> _redTeam;
        private List<Ship> _blueTeam;
        private PhysicsEngine _physicsEngine;
        
        private static  Random rng = new Random();
        private Ship _lockedShip;

        public event EventHandler<FlavourTextEventArgs> FlavourTextEventHandler;

        public uint CurrentRound { get; private set; }
        public uint RoundLimit { get; private set; }

        public static int Random(int i, int j)
        {
            return rng.Next(i, j);
        }

        public static int Random(int i)
        {
            return rng.Next(i);
        }

        public bool IsGameFinished()
        {
            return (
                _redTeam.TrueForAll(x => x.IsDestroyed()) || 
                _blueTeam.TrueForAll(x => x.IsDestroyed()) ||
                CurrentRound >= RoundLimit);
        }

        public void StartNewGame(List<Ship> redTeam, List<Ship> blueTeam, uint roundLimit)
        {
            if (redTeam.Count == 0)
                throw new ArgumentException("Each team must have at least one ship! Red team does not :(");
            if (blueTeam.Count == 0)
                throw new ArgumentException("Each team must have at least one ship! Blue team does not :(");
            if (roundLimit == 0)
                throw new ArgumentOutOfRangeException("There must be at least one round in each game. Don't set roundlimit to 0 :(");

            _redTeam = redTeam;
            _blueTeam = blueTeam;
            RoundLimit = roundLimit;
            CurrentRound = 0;

            _physicsEngine = new PhysicsEngine();
            _redTeam.ForEach(x => {
                _physicsEngine.Register(x);
                x.FlavourTextEventHandler += OnFlavourText;
            });
            _blueTeam.ForEach(x => {
                _physicsEngine.Register(x);
                x.FlavourTextEventHandler += OnFlavourText;
            });
        }
        public int GetWhichTeamWon()
        {
            if (!IsGameFinished())
            {
                throw new Exception(" You can't ask who won if the game is not finished!");
            }
            var redTeamDestroyed = _redTeam.TrueForAll(x => x.IsDestroyed());
            var blueTeamDestroyed = _blueTeam.TrueForAll(x => x.IsDestroyed());

            if ((blueTeamDestroyed && redTeamDestroyed) ||(!blueTeamDestroyed && !redTeamDestroyed))
                return -1;
            if (blueTeamDestroyed)
                return 1;
            return 2;
        }

        public void RunOneRound(Order order) 
        {
            if ( CurrentRound >= RoundLimit)
            {
                throw new Exception("The Round limit has been reached. The game is over! Stop trying to run it! Check the current round next time!");
            }
            CurrentRound += 1;
            Console.WriteLine("Turn: " + CurrentRound);
            _physicsEngine.Update(CurrentRound);
            _redTeam.ForEach(x => x.Update(CurrentRound));
            _blueTeam.ForEach(x => x.Update(CurrentRound));


            //Todo: actual commnds and ai.
            DoOrder(order, _redTeam[0]);
            for ( int i = 1; i < _redTeam.Count; i++)
            {
                if (_redTeam[i].IsDestroyed())
                    continue;
                var target = _blueTeam[rng.Next(_blueTeam.Count)];
                _redTeam[i].ShootAt(target);
            }
            Console.WriteLine();
            _blueTeam.ForEach(x => {
                var target = _redTeam[rng.Next(_redTeam.Count)];
                x.ShootAt(target);
                if (x.DistanceTo(target) > 100)
                {
                  //  x.AllStop();
                }
                else
                {
                   // x.SetCourse(rng.Next(360));
                  //  x.Throttle = rng.Next(100);
                }
            });

            Console.WriteLine();


            // this will probably get moved outside the engine. One should probably use the scan command etc to view status.
            _redTeam.ForEach(x => Console.WriteLine(x));
            Console.WriteLine();
            _blueTeam.ForEach(x => Console.WriteLine(x));

        }

        private void DoOrder(Order order, Ship ship)
        {
            if (ship.IsDestroyed()) // you are dead... no orders for you :)
                return;
            switch (order.Type)
            {
                case OrderType.SET_COURSE:
                    var setCourseOrder = (SetCourseOrder)order;
                    ship.SetCourse(setCourseOrder.AngleInDegrees);
                    OnFlavourText(this, new FlavourTextEventArgs { name = ship.GetName(), message = "Setting course for heading "+ setCourseOrder.AngleInDegrees });
                    break;
                case OrderType.SET_THROTTLE:
                    var setThrottleOrder = (SetThrottleOrder)order;
                    ship.Throttle = setThrottleOrder.ThrottlePercent;
                    OnFlavourText(this, new FlavourTextEventArgs { name = ship.GetName(), message = "Setting throttle to  " + setThrottleOrder.ThrottlePercent });
                    break;
                case OrderType.LOCK:
                    var lockOrder = (LockOrder)order;
                    var shipToLockOn = _blueTeam.Where(x => x.GetName() == lockOrder.ShipToLockOn).FirstOrDefault();
                    if (shipToLockOn == null)
                        shipToLockOn = _redTeam.Where(x => x.GetName() == lockOrder.ShipToLockOn).FirstOrDefault();
                    if (shipToLockOn == null)
                    {
                        OnFlavourText(this, new FlavourTextEventArgs { name = ship.GetName(), message = "No ship found with that name, Sir!"});
                        break;
                    }
                    _lockedShip = shipToLockOn;
                    OnFlavourText(this, new FlavourTextEventArgs { name = ship.GetName(), message = "Locking weapons on to the " + shipToLockOn.GetName() });
                    break;
                case OrderType.FIRE:
                    if ( _lockedShip == null)
                    {
                        OnFlavourText(this, new FlavourTextEventArgs { name = ship.GetName(), message = "At which ship, Sir?"});
                        break;
                    }
                    OnFlavourText(this, new FlavourTextEventArgs { name = ship.GetName(), message = "Firing weapons!" });
                    ship.ShootAt(_lockedShip);
                    break;
                default:
                    OnFlavourText(this, new FlavourTextEventArgs { name=ship.GetName(), message="Sorry, what was that, Captain?" });
                    break;
            }
        }

        private void OnFlavourText(object sender, FlavourTextEventArgs e)
        {
            FlavourTextEventHandler?.Invoke(sender, e);
        }
    }
}
