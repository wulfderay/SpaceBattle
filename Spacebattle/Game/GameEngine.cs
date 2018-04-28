using Spacebattle.Behaviours;
using Spacebattle.entity;
using Spacebattle.entity.parts.Weapon;
using Spacebattle.Entity;
using Spacebattle.orders;
using Spacebattle.physics;
using System;
using System.Collections.Generic;
using System.Linq;
using static Spacebattle.Game.GameEngineEventArgs;
using static Spacebattle.orders.Order;

namespace Spacebattle.Game
{
    public class GameEngine:IFlavourTextProvider, IGameState
    {
        public const int GAIA_TEAM = -1;
        public const int RED_TEAM = 1;
        public const int BLUE_TEAM = 0;

        private List<IShip> _redTeam; // do we want to split these up? dunno anymore.
        private List<IShip> _blueTeam;
        private PhysicsEngine _physicsEngine;
        
        private static  Random rng = new Random();
        

        public event EventHandler<FlavourTextEventArgs> FlavourTextEventHandler;
        public event EventHandler<ViewEventArgs> ViewEventHandler;

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

        public void StartNewGame(List<IShip> redTeam, List<IShip> blueTeam, uint roundLimit)
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
            var playerShip = _redTeam[0];
            _physicsEngine = new PhysicsEngine();
            _redTeam.ForEach(x => {
                _physicsEngine.Register(x);
                x.Team = RED_TEAM;
                if (x != playerShip)
                    x.AddBehaviour(new BasicAiBehaviour(x));
                x.gameState = this;
                x.FlavourTextEventHandler += OnRedFlavourText; // should this just be a gameengine event?
                x.GameEngineEventHandler += (sender, args) => OnGameEngineEvent(sender, args);
                x.Position = new Vector2d(rng.Next(0, 200), rng.Next(0, 200));
            });
            _blueTeam.ForEach(x => {
                x.Team = BLUE_TEAM;
                _physicsEngine.Register(x);
                x.AddBehaviour(new BasicAiBehaviour(x));
                x.gameState = this;
                x.FlavourTextEventHandler += OnBlueFlavourText;
                x.GameEngineEventHandler += (sender, args) => OnGameEngineEvent(sender, args);
                x.Position = new Vector2d(rng.Next(500, 700), rng.Next(500, 700));
            });
        }

        private void OnGameEngineEvent(object sender, GameEngineEventArgs e)
        {
            switch ( e.Type)
            {
                case GameEngineEventType.DAMAGE:
                    doDamageEvent((DamageEvent)e);
                    break;
                case GameEngineEventType.DESTROYED:
                    doDestroyedEvent((DestroyEvent)e);
                    break;
                case GameEngineEventType.SPAWN:
                    doSpawnEvent((SpawnEvent)e);
                    break;
            }
        }

        private void doSpawnEvent(SpawnEvent e)
        {
            throw new NotImplementedException();
        }

        private void doDestroyedEvent(DestroyEvent e)
        {
            _physicsEngine.DeRegister(e.Entity);
        }

        private void doDamageEvent(DamageEvent e)
        {// TODO: queue this and do all damage in the same step.
            // Also, do line of sight checks etc.
            e.Entity.Damage(e.DamageSource);
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
            // also, it might be a good idea to queue up actions and results and apply them all at one in order to be failr ...
            DoOrder(order, _redTeam[0]);
            for ( var i = 1; i < _redTeam.Count; i++)
            {
                _redTeam[i].ExecuteBehaviours();
            }
            _blueTeam.ForEach(x =>
            {
                x.ExecuteBehaviours();
            });
            CurrentRound += 1;
            _physicsEngine.Update(CurrentRound);
            _redTeam.ForEach(x => x.Update(CurrentRound));
            _blueTeam.ForEach(x => x.Update(CurrentRound));

        }

        private void DoOrder(Order order, IControllableEntity ship)
        {
            if (ship.IsDestroyed()) // you are dead... no orders for you :)
                return;
            switch (order.Type)
            {
                case OrderType.HELM:
                    var setCourseOrder = (HelmOrder)order;
                    if (setCourseOrder.AngleInDegrees != null)
                    {
                        ship.SetCourse((float)setCourseOrder.AngleInDegrees);
                        OnFlavourText(this, new FlavourTextEventArgs { name = ship.Name, message = "Setting course for heading " + setCourseOrder.AngleInDegrees });
                    }
                    if (setCourseOrder.ThrottlePercent != null)
                    {
                        ship.SetThrottle((float)setCourseOrder.ThrottlePercent);
                        OnFlavourText(this, new FlavourTextEventArgs { name = ship.Name, message = "Setting throttle to  " + setCourseOrder.ThrottlePercent });
                    }
                    break;
                case OrderType.LOCK:
                    var lockOrder = (LockOrder)order;
                    var shipToLockOn = _blueTeam.FirstOrDefault(x => x.Name.ToLower()== lockOrder.ShipToLockOn.ToLower());
                    if (shipToLockOn == null)
                        shipToLockOn = _redTeam.FirstOrDefault(x => x.Name.ToLower() == lockOrder.ShipToLockOn.ToLower());
                    if (shipToLockOn == null)
                    {
                        OnFlavourText(this, new FlavourTextEventArgs { name = ship.Name, message = "No ship found with that name, Sir!"});
                        break;
                    }
                    if ( lockOrder.WeaponName != null)
                    {
                        ship.LockOn(shipToLockOn, lockOrder.WeaponName);
                        break;
                    }
                    if (lockOrder.WeaponType != null)
                    {
                        ship.LockOn(shipToLockOn, (WeaponType)lockOrder.WeaponType);
                        break;
                    }
                    ship.LockOn(shipToLockOn);
                    break;
                case OrderType.FIRE:
                    var fireOrder = (FireOrder)order;
                    if (fireOrder.WeaponType != null)
                    {
                        ship.Shoot((WeaponType)fireOrder.WeaponType);
                        break;
                    }
                    if (fireOrder.WeaponName != null)
                    {
                        ship.Shoot(fireOrder.WeaponName);
                        break;
                    }
                    ship.Shoot();
                    break;
                case OrderType.SCAN: // this shouldn't take up a turn.
                    var scanOrder = (ScanOrder)order;
                    var shipToScan = _blueTeam.FirstOrDefault(x => x.Name.ToLower() == scanOrder.ShipToScan.ToLower());
                    if (shipToScan == null)
                        shipToLockOn = _redTeam.FirstOrDefault(x => x.Name.ToLower() == scanOrder.ShipToScan.ToLower());
                    if (shipToScan == null)
                    {
                        OnFlavourText(this, new FlavourTextEventArgs { name = ship.Name, message = "No ship found with that name, Sir!" });
                        break;
                    }
                    OnViewEvent(this, ViewEventArgs.Scan(shipToScan));
                   
                    OnFlavourText(this, new FlavourTextEventArgs { name = ship.Name, message = "Targetting scanners on the " + shipToScan.Name });
                    break;
                case OrderType.ALL_STOP:
                    OnFlavourText(this, new FlavourTextEventArgs { name = ship.Name, message = "Stopping all motion, Capitain." });
                    ship.AllStop();
                    break;
                default:
                    OnFlavourText(this, new FlavourTextEventArgs { name=ship.Name, message="Sorry, what was that, Captain?" });
                    break;
            }
        }


        private void OnViewEvent(object sender, ViewEventArgs e)
        {
            ViewEventHandler?.Invoke(sender, e);
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

        public IEnumerable<IDamageableEntity> GetDamageableEntities()
        {
            return new List<IDamageableEntity>().Concat(_redTeam).Concat(_blueTeam);
        }
    }
}
