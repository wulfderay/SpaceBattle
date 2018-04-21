using Spacebattle.Damage;

namespace Spacebattle.Game
{
    public class DamageEvent: GameEngineEventArgs
    {
        public DamageEvent() { Type = GameEngineEventType.DAMAGE; }
        public DamageSource DamageSource { get; internal set; }
        public IDamageableEntity Entity { get; internal set; }
    }
}