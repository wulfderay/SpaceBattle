using Spacebattle.entity;
using Spacebattle.entity.parts.Weapon;
using Spacebattle.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            OnGameEngineEvent(this, GameEngineEventArgs.Spawn(new Torpedo(_target)));
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
