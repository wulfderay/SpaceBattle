using Spacebattle.entity;
using Spacebattle.Game;
using System.Linq;

namespace Spacebattle.Behaviours
{
    class BasicAiBehaviour : IBehaviour
    {
        private IShip _ship;
        private IDamageableEntity _target;

        public BasicAiBehaviour(IShip ship)
        {
            _ship = ship;
        }
        public void Execute()
        {
            if (_ship.IsDestroyed())
                return;
            if (_target == null || _target.IsDestroyed()) // the way we hanle this is with behaviours... they can add he state needed.
            {
                var enemies = _ship.GetVisibleEntites().Where(x => x.Team != _ship.Team && x.Team != GameEngine.GAIA_TEAM).ToList(); 
                var _shipToKill = enemies[GameEngine.Random(0, enemies.Count())];
                _ship.LockOn(_shipToKill);
                _target = _shipToKill;
                return; // don't give unfair turn advantage
            }
            else
            {
                _ship.Shoot();
            }

            if (_ship.Position.DistanceTo(_target.Position) > 190)
            {
                if (_ship.Position.DistanceTo(_target.Position) > 200)
                {
                    _ship.SetCourse(_ship.Position.DirectionInDegreesTo(_target.Position));
                    _ship.SetThrottle(GameEngine.Random(0,10));
                }
                else
                {
                    _ship.AllStop();
                }
            }
            else
            {
                _ship.SetCourse(GameEngine.Random(0, 360));
                _ship.SetThrottle(GameEngine.Random(0, 10));
            }
        }


    }
}
