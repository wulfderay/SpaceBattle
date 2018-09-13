
using Spacebattle.physics;

namespace Spacebattle.Damage
{
    public class DamageSource
    {
        public Vector2d Origin;
        public DamageType DamageType;
        public float Magnitude;
    }

    public enum DamageType
    {
        CONCUSSIVE=0, // weak against sheilds, strong against hull
        FIRE,  // continues to do damage over time until put out. Doubles kills on crew decks
        RADIATION, // a miasma of pain that slowly damages every subpart, ignores sheilds
        PIERCING, // small damage to single part, ignores sheilds
        EXPLOSIVE, // chance to destroy a damaged part or ship
        SCATTERSHOT, // damages several things at once
        DRAINING //double damage to sheilds, half damage to hull.
    }
}
