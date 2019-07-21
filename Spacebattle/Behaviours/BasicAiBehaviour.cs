using Spacebattle.entity;
using Spacebattle.Entity.parts.Weapon;
using Spacebattle.Game;
using Spacebattle.orders;
using System.Linq;

namespace Spacebattle.Behaviours
{
    class BasicAiBehaviour : IBehaviour
    {
        private Ship _ship;
        private IDamageableEntity _target;
        

        public BasicAiBehaviour(Ship ship)
        {
            _ship = ship;
        }

        public Order GetNextOrder()
        {
            if (_ship.IsDestroyed())
                return Order.NullOrder();
            var unloadedTorpedoTubes = _ship.Weapons.Where(weapon => weapon is TorpedoTube && (weapon as TorpedoTube).LoadingState == LoadState.UNLOADED);
            if (unloadedTorpedoTubes.Any())
            {
                return Order.Load(""); // TODO: flesh out load more.
            }
            if (_target == null || _target.IsDestroyed()) // the way we handle this is with behaviours... they can add he state needed.
            {
                var enemies = _ship.GetVisibleEntites().Where(x => x.Team != _ship.Team && x.Team != GameEngine.GAIA_TEAM).ToList();
                _target = enemies[GameEngine.Random(0, enemies.Count())];
                return Order.Lock(_target);
            }
            
            if (_ship.Weapons.Where(weapon => weapon.TargetIsInRange() && weapon.IsReadyToFire()).Any())
            {
                return Order.Fire();
            }

            // no weapons in range.
            if (!_ship.Weapons.Where(weapon => weapon.TargetIsInRange()).Any())
            {
                return Order.SetCourse(_ship.Position.DirectionInDegreesTo(_target.Position), GameEngine.Random(1, 20));
            }

            if ( _ship.Velocity.Magnitude > 100)
            {

                return Order.AllStop();
            }
            return Order.SetCourse(GameEngine.Random(0, 360), GameEngine.Random(1, 20));

        }
    }
}
