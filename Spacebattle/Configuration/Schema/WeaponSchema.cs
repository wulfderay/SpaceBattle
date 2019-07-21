using Spacebattle.entity.parts.Weapon;

namespace Spacebattle.Configuration.Schema
{
    public class WeaponSchema
    {
        public string WeaponType { get;  set; }
        public string Name { get;  set; }
        public float MaxHealth { get;  set; }
        public float Mass { get;  set; }
        public float UpkeepCost { get;  set; }
        public float Power { get;  set; }
        public float Range { get;  set; }
        public uint Ammo { get;  set; }
        public string TorpedoType { get;  set; }
    }
}
