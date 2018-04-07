
using Spacebattle.physics;

namespace Spacebattle.Damage
{
    public class DamageSource
    {
        public Vector2d Origin;
        public DamageType Type;
        public float Magnitude;
    }

    public enum DamageType
    {
        CONCUSSIVE=0,
        FIRE, 
        RADIATION,
        PIERCING,
        SPLASH,
        SCATTERSHOT
    }
}
