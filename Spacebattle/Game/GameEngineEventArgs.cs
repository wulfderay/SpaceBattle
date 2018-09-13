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

        public static SpawnEvent Spawn(IGameEntity entityToSpawn)
        {
            return new SpawnEvent() { Entity = entityToSpawn }; 
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