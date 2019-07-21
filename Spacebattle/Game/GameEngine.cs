using Spacebattle.Behaviours;
using Spacebattle.entity;
using Spacebattle.Entity;
using Spacebattle.orders;
using Spacebattle.physics;
using System;
using System.Collections.Generic;
using System.Linq;
using static Spacebattle.Game.GameEngineEventArgs;

namespace Spacebattle.Game
{
    public class GameEngine : IFlavourTextProvider, IGameState
    {
        public const int GAIA_TEAM = -1;

        private PhysicsEngine _physicsEngine;

        private static Random rng = new Random();
        private List<IGameEntity> _markedForRemoval = new List<IGameEntity>();

        public List<IGameEntity> Entities { get; } = new List<IGameEntity>();
        public Ship Flagship { get; private set; }
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
            var numSurvivingTeams = Entities.Where(x => x.Team != GAIA_TEAM && !x.IsDestroyed()).Select(x => x.Team).Distinct().Count();
            return (
                numSurvivingTeams < 2 ||
                CurrentRound >= RoundLimit);
        }

        public void StartNewGame(Ship flagship, List<Ship> startingEntities,  uint roundLimit)
        {
            var numStartingTeams = startingEntities.Where(x => x.Team != GAIA_TEAM).Select(x => x.Team).Distinct().Count();
            if (numStartingTeams < 2)
                throw new ArgumentException("There must be at least 2 teams!");
            if (roundLimit == 0)
                throw new ArgumentOutOfRangeException("There must be at least one round in each game. Don't set roundlimit to 0 :(");

            RoundLimit = roundLimit;
            CurrentRound = 0;
            Flagship = flagship;
            _physicsEngine = new PhysicsEngine();
            startingEntities.ForEach(x => {
                _physicsEngine.Register(x);
                Entities.Add(x);
                if (x != Flagship)
                    x.AddBehaviour(new BasicAiBehaviour(x));
                x.gameState = this;

                x.FlavourTextEventHandler += OnFlavourText; // should this just be a gameengine event?
                
                x.GameEngineEventHandler += (sender, args) => OnGameEngineEvent(sender, args);
                x.Position = GetStartingPosition(x.Team, numStartingTeams);
                x.Update(0);
            });
            
        }

        private Vector2d GetStartingPosition(int team, int numStartingTeams)
        {
            // the idea is to create a polygon with numStartingTeams points, with edgeLength distanceBetweenTeams.
            // we then distribute the members of each team randomly around each point.
            var distanceBetweenTeams = 1000;
            var angleBetweenTeams = (Math.PI*2) / numStartingTeams;
            var polygonAngle = (Math.PI - angleBetweenTeams) / 2;
            // we can find the distance to the center of the polygon using geometry.
            var rightAngleDistance = Math.Tan(polygonAngle) * distanceBetweenTeams/2 ;
            var distanceToCenter = Math.Sqrt(
                (distanceBetweenTeams/2 * distanceBetweenTeams/2) + 
                (rightAngleDistance * rightAngleDistance));

            var centroidX = (int)(Math.Cos(angleBetweenTeams * team) * distanceToCenter);
            var centroidY =(int)( Math.Sin(angleBetweenTeams * team) * distanceToCenter);
            int spread = 300;
            return new Vector2d(rng.Next(centroidX -spread, centroidX + spread), rng.Next(centroidY - spread, centroidY + spread));
        }

        private void OnGameEngineEvent(object sender, GameEngineEventArgs e)
        {
            switch ( e.Type)
            {
                case GameEngineEventType.DAMAGE:
                    doDamageEvent((DamageEvent)e);
                    break;
                case GameEngineEventType.SPLASH_DAMAGE:
                    doSplashDamageEvent((SplashDamageEvent)e);
                    break;
                case GameEngineEventType.DESTROYED:
                    doDestroyedEvent((DestroyEvent)e);
                    break;
                case GameEngineEventType.SPAWN:
                    doSpawnEvent((SpawnEvent)e);
                    break;
            }
        }

        private void doSplashDamageEvent(SplashDamageEvent e)
        {
            foreach (var entity in Entities.Where(entity => entity.Position.DistanceTo(e.DamageSource.Origin) < e.Radius))
            {
                entity.Damage(e.DamageSource);
            }
        }

        private void doSpawnEvent(SpawnEvent e)
        {
            Entities.Add(e.Entity);
            e.Entity.GameEngineEventHandler += (sender, args) => OnGameEngineEvent(sender, args);
            if ( e.Entity is Ship)
            {
                (e.Entity as Ship).gameState = this;
                (e.Entity as Ship).FlavourTextEventHandler += OnFlavourText;
            }
            e.Entity.Position = e.Position;
            e.Entity.Velocity = e.Velocity;
            e.Entity.Orientation = e.Orientation;
            _physicsEngine.Register(e.Entity);
        }

        private void doDestroyedEvent(DestroyEvent e)
        {
            _markedForRemoval.Add(e.Entity);
            // hmm. Can't add flavour text here.. :/
            e.Entity.GameEngineEventHandler -= (sender, args) => OnGameEngineEvent(sender, args);
            // we probably want to remove from drawing and from updating, too....
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
            if (Entities.Any(x => x.Team != GAIA_TEAM && !x.IsDestroyed()))
            {
                var SurvivingTeam = Entities.First(x => x.Team != GAIA_TEAM && !x.IsDestroyed()).Team;

                return SurvivingTeam;
            }
            return GAIA_TEAM;
        }

        public void RunOneRound(Order order) 
        {
            if ( CurrentRound >= RoundLimit)
            {
                throw new Exception("The Round limit has been reached. The game is over! Stop trying to run it! Check the current round next time!");
            }

            //Todo: actual commnds and ai.
            // also, it might be a good idea to queue up actions and results and apply them all at one in order to be failr ...
            (Flagship as IControllableEntity).DoOrder(order);

            Entities.ToList().ForEach(x => {
                if ( x is IControllableEntity && x != Flagship)
                {
                    (x as IControllableEntity).DoOrder(x.ExecuteBehaviours());
                }
            });

            CurrentRound += 1;
            _physicsEngine.Update(CurrentRound);
            Entities.ForEach(x => {
                if (!x.IsDestroyed())
                    x.Update(CurrentRound);
            });
            // clean up destroyed stuff.
            _markedForRemoval.ForEach(x => Entities.Remove(x));
            _markedForRemoval.Clear();
        }


        private void OnViewEvent(object sender, ViewEventArgs e)
        {
            ViewEventHandler?.Invoke(sender, e);
        }

       
        private void OnFlavourText(object sender, FlavourTextEventArgs e)
        {
            FlavourTextEventHandler?.Invoke(sender, e);
        }

        public IEnumerable<IDamageableEntity> GetDamageableEntities()
        {
            return Entities.Where(x => x is IDamageableEntity);
        }
    }
}
