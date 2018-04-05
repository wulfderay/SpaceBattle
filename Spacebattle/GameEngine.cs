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
                x.FlavourTextEventHandler += OnRedFlavourText;
                x.Position = new Vector2d(rng.Next(0, 200), rng.Next(0, 200));
            });
            _blueTeam.ForEach(x => {
                _physicsEngine.Register(x);
                x.FlavourTextEventHandler += OnBlueFlavourText;
                x.Position = new Vector2d(rng.Next(500, 700), rng.Next(500, 700));
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

            //Todo: actual commnds and ai.
            DoOrder(order, _redTeam[0]);
            for ( var i = 1; i < _redTeam.Count; i++)
            {
                DoAIOrder(_redTeam[i], _blueTeam);
            }
            _blueTeam.ForEach(x =>
            {
                DoAIOrder(x, _redTeam);
            });
            CurrentRound += 1;
            _physicsEngine.Update(CurrentRound);
            _redTeam.ForEach(x => x.Update(CurrentRound));
            _blueTeam.ForEach(x => x.Update(CurrentRound));

        }

        private void DoAIOrder(Ship ship, List<Ship> enemies)
        {
            if (ship.IsDestroyed())
                return;
            if (ship.LockedShip == null || ship.LockedShip.IsDestroyed())
            {
                ship.LockOn(enemies[rng.Next(enemies.Count)]);
                return; // don't give unfair turn advantage
            }
            else
            {
                ship.ShootAt(ship.LockedShip);
            }
            
            if (ship.DistanceTo(ship.LockedShip) > 190)
            {
                if (ship.DistanceTo(ship.LockedShip) > 200)
                {
                    ship.SetCourse(ship.DirectionInDegreesTo(ship.LockedShip));
                    ship.Throttle = rng.Next(10);
                }
                else
                {
                    ship.AllStop();
                }
            }
            else
            {
                ship.SetCourse(rng.Next(360));
                ship.Throttle = rng.Next(10);
            }
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
                    var shipToLockOn = _blueTeam.FirstOrDefault(x => x.GetName() == lockOrder.ShipToLockOn);
                    if (shipToLockOn == null)
                        shipToLockOn = _redTeam.FirstOrDefault(x => x.GetName() == lockOrder.ShipToLockOn);
                    if (shipToLockOn == null)
                    {
                        OnFlavourText(this, new FlavourTextEventArgs { name = ship.GetName(), message = "No ship found with that name, Sir!"});
                        break;
                    }
                    ship.LockOn(shipToLockOn);
                    OnFlavourText(this, new FlavourTextEventArgs { name = ship.GetName(), message = "Locking weapons on to the " + shipToLockOn.GetName() });
                    break;
                case OrderType.FIRE:
                    if ( ship.LockedShip == null)
                    {
                        OnFlavourText(this, new FlavourTextEventArgs { name = ship.GetName(), message = "At which ship, Sir?"});
                        break;
                    }
                    OnFlavourText(this, new FlavourTextEventArgs { name = ship.GetName(), message = "Firing weapons!" });
                    ship.ShootAt(ship.LockedShip);
                    break;
                case OrderType.ALL_STOP:
                    OnFlavourText(this, new FlavourTextEventArgs { name = ship.GetName(), message = "Stopping all motion, Capitain." });
                    ship.AllStop();
                    break;
                default:
                    OnFlavourText(this, new FlavourTextEventArgs { name=ship.GetName(), message="Sorry, what was that, Captain?" });
                    break;
            }
        }

        private void OnBlueFlavourText(object sender, FlavourTextEventArgs e)
        {
            e.team = 0;
            FlavourTextEventHandler?.Invoke(sender, e);

        }

        private void OnRedFlavourText(object sender, FlavourTextEventArgs e)
        {
            e.team = 1;
            FlavourTextEventHandler?.Invoke(sender, e);
        }
        private void OnFlavourText(object sender, FlavourTextEventArgs e)
        {
            e.team = -1; //gaia
            FlavourTextEventHandler?.Invoke(sender, e);
        }
    }
}
