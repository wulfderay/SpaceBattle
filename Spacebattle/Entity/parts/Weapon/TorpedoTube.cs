using Spacebattle.entity;
using Spacebattle.entity.parts.Weapon;
using Spacebattle.Game;
using System;

namespace Spacebattle.Entity.parts.Weapon
{
    class TorpedoTube : ShipPart, IWeapon
    {
        private IDamageableEntity _target;
        private const uint LOADING_ROUNDS = 4;
        private Func<IDamageableEntity, Torpedo> _torpSpawnFunc;
        public TorpedoTube(string name, float maxHealth, float mass, float upkeepCost, Func<IDamageableEntity, Torpedo> torpSpawnFunc) : base(name, maxHealth, mass, upkeepCost)
        {
            _torpSpawnFunc = torpSpawnFunc;
        }

        public LoadState LoadingState { get; private set; }
        public uint TurnsUntilLoaded { get; private set; } = 0;
        public uint TotalLoadingTurns { get => LOADING_ROUNDS; }

        public void Fire()
        {
            if (IsDestroyed())
                return;
            if (LoadingState != LoadState.LOADED)
            {
                OnFlavourText(_name, "Torpedo is not loaded!");
                return;
            }
            OnFlavourText(_name, "Firing Torpedo!");
            LoadingState = LoadState.LOADING;
            TurnsUntilLoaded = LOADING_ROUNDS;

            OnGameEngineEvent(this, GameEngineEventArgs.Spawn(_torpSpawnFunc(_target), Parent.Position));
        }
        
        public IDamageableEntity GetLockTarget()
        {
            return _target;
        }

        public WeaponType GetWeaponType()
        {
            return WeaponType.TORPEDO;
        }

        public bool IsReadyToFire()
        {
            return LoadingState == LoadState.LOADED;
        }

        public void Lock(IDamageableEntity target)
        {
            _target = target;
        }

        public bool TargetIsInRange()
        {
            return true; //it actually depends on the type of torpedo...
        }

        public void Load()
        {
            LoadingState = LoadState.LOADING;
            TurnsUntilLoaded = LOADING_ROUNDS;
        }
        

        public new void Update(uint roundNumber)
        {
            base.Update(roundNumber);
            if (IsDestroyed())
                return;
            if (LoadingState == LoadState.LOADING)
            {
                TurnsUntilLoaded--;
                if (TurnsUntilLoaded <= 0)
                    LoadingState = LoadState.LOADED;
            }
        }
    }
}
