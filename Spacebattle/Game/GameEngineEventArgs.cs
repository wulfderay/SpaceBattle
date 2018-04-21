using Spacebattle.Damage;
using Spacebattle.entity;

namespace Spacebattle.Game
{
    public class GameEngineEventArgs
    {
        public enum GameEngineEventType
        {
            SPAWN=0,
            DAMAGE,
            DESTROYED
        }
        public GameEngineEventType Type;

        public static SpawnEvent Spawn(IEntity entityToSpawn)
        {
            return new SpawnEvent() { Entity = entityToSpawn }; 
        }

        public static DestroyEvent Destroy(IEntity entityToDestroy)
        {
            return new DestroyEvent() { Entity = entityToDestroy };
        }
        public static DamageEvent Damage(IDamageableEntity entityToDamage, DamageSource damageSource)
        {
            return new DamageEvent() { Entity = entityToDamage, DamageSource = damageSource };
        }
    }

    
}