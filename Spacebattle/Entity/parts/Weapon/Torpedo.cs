

using System;
using System.Collections.Generic;
using Spacebattle.Behaviours;
using Spacebattle.Damage;
using Spacebattle.entity;
using Spacebattle.Game;
using Spacebattle.physics;

namespace Spacebattle.Entity.parts.Weapon
{
    class Torpedo : GameEntity, IControllableEntity
    {
        // this isn't quite right... A torpedo needs to know it's target, sure.. but the target maybe? should be able to be changed.
        // it probably doesn't belong in the constructor.
        // It _does_ need a payload and health and speed and other fun stuff like that...

        private IDamageableEntity target;
        private bool _isDestroyed;
        private float _throttle;
        private int _TTL;
        private float _proximityFuse;
        private float _damage;
        private float _damageRadius;
        private float _enginePower = 50; // I guess.. I don't know.
        public Torpedo(IDamageableEntity target, int ttl, float proximityFuse, float damage, float damageRadius)
        {
            Team = GameEngine.GAIA_TEAM;
            Name = "Torpedo";
            _TTL = ttl;
            _proximityFuse = proximityFuse;
            this.target = target;
            _damage = damage;
            _damageRadius = damageRadius;
            Mass = 1;
            AddBehaviour(new Pursue(this, target, 0, 100));

        }
        public void AllStop()
        {
            Velocity = Vector2d.Zero;
            SetThrottle(0);
        }

        public override void Damage(DamageSource damage) // any damage at this point should destroy it.
        {
            if (_isDestroyed)
                return;
            BlowUp();
        }

        public float GetMaxAcceleration()
        {
            throw new NotImplementedException();
        }

        public float GetThrottle()
        {
            return _throttle;
        }

        public List<IDamageableEntity> GetVisibleEntites()
        {
            return new List<IDamageableEntity>() { target }; // no sensors yet I guess.
        }

        public override bool IsDestroyed()
        {
            return _isDestroyed;
        }

        public void SetCourse(float angle)
        {
            Orientation = angle;
        }

        public void SetThrottle(float percent)
        {
            _throttle = percent;
        }

        public override void Update(uint roundNumber)
        {
            if (_TTL <= 0 || target.Position.DistanceTo(this.Position) < _proximityFuse)
            {
                BlowUp();
            }
            //move the torp
            var impulse = Vector2d.fromAngleDegrees(Orientation) * (_throttle/100 * _enginePower);
            ApplyImpulse(impulse);
            _TTL--;
        }

        private void BlowUp()
        {
            _isDestroyed = true;
            OnGameEngineEvent(this, GameEngineEventArgs.Destroy(this)); // destroy first so we don't get in a infinite loop of destruction.
            OnGameEngineEvent(this, GameEngineEventArgs.SplashDamage(_damageRadius, new DamageSource()
            {
                Magnitude = _damage,
                Origin = Position,
                DamageType = DamageType.EXPLOSIVE
            })); 
        }
    }
}
