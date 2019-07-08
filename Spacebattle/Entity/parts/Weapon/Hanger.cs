using Spacebattle.entity;
using Spacebattle.entity.parts.Weapon;
using Spacebattle.Game;
using System;

namespace Spacebattle.Entity.parts.Weapon
{
    class Hanger : ShipPart
    {
        private Func<Ship> _shipCreationFunc;

        public Hanger(string name, float maxHealth, float mass, float upkeepCost, Func<Ship> shipCreationFunc) : base(name, maxHealth, mass, upkeepCost)
        {
            _shipCreationFunc = shipCreationFunc;
        }

        public void Fire()
        {
            OnFlavourText(_name, "Launching ship from hanger!");
            var shipToSpawn = _shipCreationFunc();
            shipToSpawn.Team = Parent.Team;
            OnGameEngineEvent(this, GameEngineEventArgs.Spawn( shipToSpawn , Parent.Position));
        }

        public IDamageableEntity GetLockTarget()
        {
            return null;
        }

        public WeaponType GetWeaponType()
        {
            return WeaponType.HANGER;
        }

        public void Lock(IDamageableEntity target)
        {
            return; // nothing to lock on to. Honesty, this is more of a part than a weapon.
        }
    }
}
