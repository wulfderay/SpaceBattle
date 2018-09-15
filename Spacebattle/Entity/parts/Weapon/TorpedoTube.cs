using Spacebattle.entity;
using Spacebattle.entity.parts.Weapon;
using Spacebattle.Game;

namespace Spacebattle.Entity.parts.Weapon
{
    class TorpedoTube : ShipPart, IWeapon
    {
        private IDamageableEntity _target;

        public TorpedoTube(string name, float maxHealth, float mass, float upkeepCost) : base(name, maxHealth, mass, upkeepCost)
        {
        }

        public void Fire()
        {
            OnFlavourText(_name, "Firing Torpedo!");
            OnGameEngineEvent(this, GameEngineEventArgs.Spawn( new Torpedo(_target, 15, 200, 150, 300), Parent.Position));
        }
        
        public IDamageableEntity GetLockTarget()
        {
            return _target;
        }

        public WeaponType GetWeaponType()
        {
            return WeaponType.TORPEDO;
        }

        public void Lock(IDamageableEntity target)
        {
            _target = target;
        }
    }
}
