using System;
using Spacebattle.Damage;
using Spacebattle.entity;
using Spacebattle.physics;

namespace Spacebattle.Game
{
    public class GameEngineEventArgs
    {
        public enum GameEngineEventType
        {
            SPAWN=0,
            DAMAGE,
            SPLASH_DAMAGE,
            DESTROYED
        }
        public GameEngineEventType Type;

        public static SpawnEvent Spawn(IGameEntity entityToSpawn, Vector2d position = null, Vector2d velocity = null, float orientation = 0)
        { 
        
            return new SpawnEvent() { Entity = entityToSpawn, Position = position?? Vector2d.Zero, Velocity = velocity?? Vector2d.Zero, Orientation = orientation  }; 
        }

        public static DestroyEvent Destroy(IGameEntity entityToDestroy)
        {
            return new DestroyEvent() { Entity = entityToDestroy };
        }
        public static DamageEvent Damage(IDamageableEntity entityToDamage, DamageSource damageSource)
        {
            return new DamageEvent() { Entity = entityToDamage, DamageSource = damageSource };
        }

        internal static GameEngineEventArgs SplashDamage( float radius, DamageSource damageSource)
        {
            return new SplashDamageEvent() { Radius = radius, DamageSource = damageSource };
        }
    }

    
}