

using System;
using Spacebattle.Damage;
using Spacebattle.entity;

namespace Spacebattle.Entity.parts.Weapon
{
    class Torpedo : GameEntity, IDamageableEntity
    {
        private IDamageableEntity target;

        // this isn't quite right... A torpedo needs to know it's target, sure.. but the target maybe? should be able to be changed.
        // it probably doesn't belong in the constructor.
        // It _does_ need a payload and health and speed and other fun stuff like that...
        

        public Torpedo(IDamageableEntity target )
        {
            this.target = target;
        }

        public void Damage(DamageSource damage)
        {
            throw new NotImplementedException();
        }

        public bool IsDestroyed()
        {
            throw new NotImplementedException();
        }
    }
}
