using Spacebattle.Damage;

namespace Spacebattle.Game
{
    public class SplashDamageEvent: GameEngineEventArgs
    {
        public SplashDamageEvent() { Type = GameEngineEventType.SPLASH_DAMAGE; }
        public DamageSource DamageSource { get; internal set; }
        public float Radius { get; internal set; }
    }
}