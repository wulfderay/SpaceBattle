using Spacebattle.entity.parts;

using System.Collections.Generic;
using System.Linq;
using System;
using Spacebattle.physics;
using Spacebattle.Damage;
using Spacebattle.entity.parts.Weapon;
using Spacebattle.Game;
using Spacebattle.Entity;
using Spacebattle.orders;
using static Spacebattle.orders.Order;
using Spacebattle.Entity.parts.Weapon;

namespace Spacebattle.entity
{
    public class Ship : GameEntity, IGameEntity, IControllableEntity, IFlavourTextProvider, IBehave
    {
        private float _throttle;

        public event EventHandler<FlavourTextEventArgs> FlavourTextEventHandler;

        internal float ConsumePower(float powerRequested)
        {
            if (Power >= powerRequested)
            {
                Power -= powerRequested;
                return powerRequested;
            }
            var powerConsumed = Power;
            Power = 0; // consume what you can.
            return powerConsumed;
        }

        public float GetThrottle()
        {
            return _throttle;
        }

        public void SetThrottle(float percent)
        {
            if (percent < 0)
                return;
            if (percent > 100)
                return;
            _throttle = percent;
        }

        public float Power { get; private set; }

        public List<CrewDeck> CrewDecks { get; }

        public List<Engine> Engines { get; }

        public List<IWeapon> Weapons { get; }

        public List<Shield> Shields { get; }

        public List<Reactor> Reactors { get; }

        public IGameState gameState
        {
            get;

            set;
        }

        public Ship ( string name, List<Reactor> reactors, List<Shield> shields, List<IWeapon> weapons, List<Engine> engines, List<CrewDeck> crewDecks)
        {
            Name = name;
            Reactors = reactors;
            Shields = shields;
            Weapons = weapons;
            Engines = engines;
            CrewDecks = crewDecks;

            Reactors.ForEach(x => { x.FlavourTextEventHandler += OnFlavourText; x.Parent = this; x.GameEngineEventHandler += OnGameEngineEvent; });
            Shields.ForEach(x => { x.FlavourTextEventHandler += OnFlavourText; x.Parent = this; x.GameEngineEventHandler += OnGameEngineEvent; });
            Weapons.ForEach(x => { x.FlavourTextEventHandler += OnFlavourText; x.Parent = this; x.GameEngineEventHandler += OnGameEngineEvent; });
            Engines.ForEach(x => { x.FlavourTextEventHandler += OnFlavourText; x.Parent = this; x.GameEngineEventHandler += OnGameEngineEvent; });
            CrewDecks.ForEach(x => { x.FlavourTextEventHandler += OnFlavourText; x.Parent = this; x.GameEngineEventHandler += OnGameEngineEvent; });

            Mass = getTotalMass();

            Position = new Vector2d();
            Velocity = new Vector2d();
            Orientation = 0;
        }



        public override void Damage(DamageSource damage)
        {
            OnFlavourText(Name, $"{Name} was hit by {damage.DamageType}!", FlavourTextEventArgs.LEVEL_INFO);
            var residualDamage = DoShieldAbsorbtion(damage);
            
            
            if (residualDamage.Magnitude == 0)
                return;
            OnFlavourText(Name, Name + " suffered a hit to the hull!!", FlavourTextEventArgs.LEVEL_INFO);
            if ( residualDamage.DamageType == DamageType.FIRE)
            {
                OnFlavourText(Name, Name + " is on fire!");
            }
            List<IShipPart> damageableParts = getDamageableParts();
            if (damageableParts.Count == 0)
            {
                // all parts destroyed, Nothing left to damage.
                return;
            }
            var partToDamage = damageableParts[GameEngine.Random(damageableParts.Count)];
            partToDamage.Damage(damage);
            OnFlavourText(Name, $"{ residualDamage.Magnitude} damage to {partToDamage.GetName()}.");
        }

        private List<IShipPart> getDamageableParts()
        {
            return new List<IShipPart>()
                .Concat(Reactors.Where(x => !x.IsDestroyed()))
                .Concat(Engines.Where(x => !x.IsDestroyed()))
                .Concat(Shields.Where(x => !x.IsDestroyed()))
                .Concat(Weapons.Where(x => !x.IsDestroyed()))
                .Concat(CrewDecks.Where(x => !x.IsDestroyed())).ToList();
        }

        private DamageSource DoShieldAbsorbtion(DamageSource damage)
        {
            if (Shields.Count == 0)
                return damage;
            if (damage.DamageType == (DamageType.RADIATION | DamageType.PIERCING))
            {
                OnFlavourText(Name, "It passed right through the shields!");
                return damage;
            }

            DamageSource residualDamage = damage;
            foreach (var shield in Shields) 
            {
                if (residualDamage.Magnitude == 0)
                    break;
                residualDamage = shield.Absorb(residualDamage); 
            }
            OnFlavourText(Name, damage.Magnitude - residualDamage.Magnitude + " damage was absorbed by shields.");
            return new DamageSource { Magnitude = residualDamage.Magnitude, Origin = damage.Origin, DamageType = damage.DamageType };
        }

        public void AllStop()
        {
           
            if (Velocity.Magnitude < 0.2)
            {
                SetThrottle(0);
                return;
            }

            SetCourse((Vector2d.Zero - Velocity).AngleDegrees);
            SetThrottle(20);
                
        }

        public override bool IsDestroyed()
        {
            if (CrewDecks.Select(x => (int)x.GetCrew()).Sum() == 0)
                return true; // probably not exactly what we want. We will probably want a difference betweeen destroyed and derelict.
            foreach (var part in Reactors)
            {
                if (!part.IsDestroyed())
                    return false;
            }
            foreach (var part in Shields)
            {
                if (!part.IsDestroyed())
                    return false;
            }
            foreach (var part in Weapons)
            {
                if (!part.IsDestroyed())
                    return false;
            }
            foreach (var part in Engines)
            {
                if (!part.IsDestroyed())
                    return false;
            }
            
            return true;
        }

        public override void Update(uint roundNumber)
        {
            if (IsDestroyed())
                return;
            // this would be where we would do repairs, regen shields, move the ship etc.
            
            Power = Reactors.Select(x => x.Produce()).Sum();

            moveShip();
            
            foreach (var crewDeck in CrewDecks)
            {
                repairARandomPartThatNeedsIt(crewDeck);
            }

            // update parts
            foreach (var part in CrewDecks)
            {
                part.Update(roundNumber);
            }
            foreach (var part in Reactors)
            {
                part.Update(roundNumber);
            }
            foreach (var part in Engines)
            {
                part.Update(roundNumber);
            }
            foreach (var part in Shields)
            {
                part.Update(roundNumber);
            }
            foreach (var part in Weapons)
            {
                part.Update(roundNumber);
            }
            foreach (var shield in Shields)
                Power = shield.Regen(Power);

        }

        private void spendPower()
        {
            // currently the only thing that this does is regen the shields or not.
            // change to for loop so we have more control owver what gets power when and what happens if there isn't enough power.
            Power -=( Reactors.Select(x => x.GetUpkeepCost()).Sum() +
               Shields.Select(x => x.GetUpkeepCost()).Sum() +
               Weapons.Select(x => x.GetUpkeepCost()).Sum() +
               Engines.Select(x => x.GetUpkeepCost()).Sum() +
               CrewDecks.Select(x => x.GetUpkeepCost()).Sum());
            if (Power < 0)
                Power = 0;
            foreach (var shield in Shields)
                Power = shield.Regen(Power);
        }

        public void LockOn(IDamageableEntity ship, WeaponType weaponType)
        {
            if ( ship != this) // stop hitting yourself! Stop hitting yourself!
            {
                var weaponsToLock = getWeaponsByType(weaponType).ToList();
                if (!weaponsToLock.Any())
                {
                    OnFlavourText(Name, "We don't have any weapons of that type, Sir!");
                    return;
                }
                weaponsToLock.ForEach(x => x.Lock(ship));
                OnFlavourText(this, new FlavourTextEventArgs { name = Name, message = "Locking weapons on to the " + ship.Name , level = FlavourTextEventArgs.LEVEL_INTERNAL, team = Team});
            }
        }
        public void LockOn(IDamageableEntity ship, string weaponName)
        {
            if (ship != this) // stop hitting yourself! Stop hitting yourself!
            {
                var weaponsToLock = getWeaponsByName(weaponName).ToList();
                if (!weaponsToLock.Any())
                {
                    OnFlavourText(Name, "Lock the " + weaponName + "?!");
                    return;
                }
                weaponsToLock.ForEach(x => x.Lock(ship));
                OnFlavourText(this, new FlavourTextEventArgs { name = Name, message = "Locking weapons on to the " + ship.Name , level = FlavourTextEventArgs.LEVEL_INTERNAL, team = Team });
            }
        }

        public void LockOn(IDamageableEntity ship)
        {
            if (ship != this) // stop hitting yourself! Stop hitting yourself!
            {
                foreach (var weapon in Weapons)
                {
                    weapon.Lock(ship);
                }
            }
            OnFlavourText(this, new FlavourTextEventArgs { name = Name, message = "Locking weapons on to the " + ship.Name, level = FlavourTextEventArgs.LEVEL_INTERNAL, team = Team });
        }


        public void Shoot(WeaponType weaponType)
        {
            var weaponsToFire = getWeaponsByType(weaponType).ToList();
            if (!weaponsToFire.Any())
            {
                OnFlavourText(Name, "We don't have any weapons of that type, Sir!");
                return;
            }
            weaponsToFire.ForEach(x => x.Fire());           
        }

        public void Shoot(string weaponName) 
        {
            var weaponsToFire = getWeaponsByName(weaponName).ToList();
            if (!weaponsToFire.Any())
            {
                OnFlavourText(Name, "Fire the " + weaponName + "?!");
                return;
            }
            weaponsToFire.ForEach(x => x.Fire());
        }

        public void Shoot() 
        {
            OnFlavourText(this, new FlavourTextEventArgs { name = Name, message = "Firing weapons!", team = Team });
            foreach (var weapon in Weapons.Where(weapon => weapon.IsReadyToFire() && !weapon.IsDestroyed() && weapon.TargetIsInRange()))
            {
                weapon.Fire();
            }

        }

        /// <summary>
        /// Gets the maximum totaly accelleration if the throttle is set to 100% on all engines.
        /// </summary>
        /// <returns></returns>
        public float GetMaxAcceleration()
        {
            // force = mass x Acceleration
            // accel = force/mass
            var force = Engines.Select(x => x.getMaxForce()).Sum();
            var mass = getTotalMass();
            return force/mass;
        }

        /// <summary>
        ///  this is really only used for initialization. Use Entity.Mass for the ship's mass as it pertains to the physics model
        /// </summary>
        /// <returns>the mass of all combined shipParts</returns>
        private float getTotalMass()
        {
            return Reactors.Select(x=>x.GetMass()).Sum() +
                Shields.Select(x => x.GetMass()).Sum() +
                Weapons.Select(x => x.GetMass()).Sum() +
                Engines.Select(x => x.GetMass()).Sum() +
                CrewDecks.Select(x => x.GetMass()).Sum();
        }

        /*
         * what commands do we want for motion?
         * 
         * Note that when we say velocity and position, for now we mean relative to the galactic centre. Planets and other gravitational bodies may come later.
         * 
         * In this model, thrust comes out the back of the ship. it is assumed that rotating the ship changes the orientation of the thrusters.
         * 
         * helm - set a course and speed, reach and hold a velocity until otherwise directed
         * course - rotates the ship to point at a direction, doesn't change thrust.
         * throttle - alters the throttle of all engines, doesn't change rotation. (note this produces constant accelleration)
         * allstop - zeroes the velocity
         * speed - sets a desired velocity
         */ 

      

        /// <summary>
        /// Rotates the ship to point in a particular direction.
        /// </summary>
        /// <param name="angle"> in degrees</param>
        public void SetCourse(float angle)
        {
            //TODO: torque, degrees to radians
            Orientation = angle;
        }

       

        private void moveShip()
        {
            if (GetThrottle() == 0)
                return;
            var enginePower = Engines.Select(x => x.Throttle(GetThrottle())).Sum();
            var impulse = Vector2d.fromAngleDegrees(Orientation) * enginePower;
            ApplyImpulse(impulse);
        }

        /// <summary>
        /// repairs some part that is damaged.
        /// </summary>
        /// <param name="crewDeck">Use this crewdeck's repair rate.</param>
        private void repairARandomPartThatNeedsIt(CrewDeck crewDeck)
        {
            List<IShipPart> damagedParts = new List<IShipPart>();
            damagedParts.AddRange( Reactors.Where(x => x.Health.Item1 < x.Health.Item2));
            damagedParts.AddRange( Engines.Where(x => x.Health.Item1 < x.Health.Item2));
            damagedParts.AddRange(Weapons.Where(x => x.Health.Item1 < x.Health.Item2));
            damagedParts.AddRange(Shields.Where(x => x.Health.Item1 < x.Health.Item2));
            damagedParts.AddRange(CrewDecks.Where(x => x.Health.Item1 < x.Health.Item2));

            if (damagedParts.Count() == 0)
                return;

            crewDeck.PerformRepair(damagedParts[GameEngine.Random(damagedParts.Count)]);
        }


        public override string ToString()
        {
            var result = "Ship:" + Name + 
                " Crew:" + CrewDecks.Select(x =>(int) x.GetCrew()).Sum()+ 
                " Mass: "+Mass+
                " Power:"+ Power + 
                " Position:[X:"+Position.X+" Y:"+Position.Y+" Heading:"+Orientation+ "° Velocity: X:"+Velocity.X+ " Y:"+ Velocity.Y +" Mag:"+ Velocity.Magnitude+"]\n";
            result +=  string.Join(" ", Reactors.Select(x => x.ToString())) + "\n";
            result += string.Join(" ", Shields.Select(x => x.ToString())) + "\n";
            result += string.Join(" ", Weapons.Select(x => x.ToString())) + "\n";
            result += string.Join(" ", Engines.Select(x => x.ToString())) + "Max :"+GetMaxAcceleration()+"\n";
            result +=  string.Join(" ", CrewDecks.Select(x => x.ToString())) + "\n";
            if (IsDestroyed())
                result += "(Destroyed)\n";
            return result;
        }

        private IEnumerable<IWeapon> getWeaponsByType(WeaponType weaponType)
        {
            return Weapons.Where(x => x.GetWeaponType() == weaponType);
        }

        private IEnumerable<IWeapon> getWeaponsByName(string weaponName)
        {
            return Weapons.Where(x => x.GetName().ToLower() == weaponName.ToLower());
        }

        private IEnumerable<IWeapon> getWeaponsByTypeOrName(WeaponType weaponType, string weaponName)
        {
            return Weapons.Where(x => x.GetName() == weaponName || x.GetWeaponType() == weaponType);
        }


        private void OnFlavourText(string name, string message, int level = FlavourTextEventArgs.LEVEL_INTERNAL)
        {
            FlavourTextEventHandler?.Invoke(this, new FlavourTextEventArgs { name = name, message = message, level = level , team = Team});
        }

        private void OnFlavourText(object sender, FlavourTextEventArgs e)
        {
            FlavourTextEventHandler?.Invoke(sender, new FlavourTextEventArgs { name = Name+"."+e.name, message = e.message , level= e.level, team = Team});
        }

        public List<IDamageableEntity> GetVisibleEntites()
        {
            // hmm... don't know how to do this yet.
            // should we always have  gameengine to see? that seems bad.
            // what if we get a list of game state every update?
            // I wish there was a a way to throw an event that would return synchronously

            return gameState.GetDamageableEntities().ToList();
        }

        public void DoOrder(Order order)
        {
            if (IsDestroyed()) // you are dead... no orders for you :)
                return;
            switch (order.Type)
            {
                case OrderType.HELM:
                    var setCourseOrder = (HelmOrder)order;
                    if (setCourseOrder.AngleInDegrees != null)
                    {
                        SetCourse((float)setCourseOrder.AngleInDegrees);
                        OnFlavourText(this, new FlavourTextEventArgs { name = Name, message = "Setting course for heading " + setCourseOrder.AngleInDegrees, team = Team, level = FlavourTextEventArgs.LEVEL_INTERNAL });
                    }
                    if (setCourseOrder.ThrottlePercent != null)
                    {
                        SetThrottle((float)setCourseOrder.ThrottlePercent);
                        OnFlavourText(this, new FlavourTextEventArgs { name = Name, message = "Setting throttle to  " + setCourseOrder.ThrottlePercent, team = Team, level = FlavourTextEventArgs.LEVEL_INTERNAL });
                    }
                    break;
                case OrderType.LOCK:
                    var lockOrder = (LockOrder)order;
                   
                    if (lockOrder.WeaponName != null)
                    {
                        LockOn(lockOrder.Target, lockOrder.WeaponName);
                        break;
                    }
                    if (lockOrder.WeaponType != null)
                    {
                        LockOn(lockOrder.Target, (WeaponType)lockOrder.WeaponType);
                        break;
                    }
                    LockOn(lockOrder.Target);
                    break;
                case OrderType.FIRE:
                    var fireOrder = (FireOrder)order;
                    if (fireOrder.WeaponType != null)
                    {
                        Shoot((WeaponType)fireOrder.WeaponType);
                        break;
                    }
                    if (fireOrder.WeaponName != null)
                    {
                        Shoot(fireOrder.WeaponName);
                        break;
                    }
                    Shoot();
                    break;
                case OrderType.SCAN: // this shouldn't take up a turn.
                    // nothing to do at this level.
                    break;
                case OrderType.ALL_STOP:
                    OnFlavourText(this, new FlavourTextEventArgs { name = Name, message = "Stopping all motion, Captain.", team = Team, level = FlavourTextEventArgs.LEVEL_INTERNAL });
                    AllStop();
                    break;
                case OrderType.LOAD:

                    var unloadedTorpedoTubes = Weapons.Where(weapon => weapon is TorpedoTube);
                    if (unloadedTorpedoTubes.Any())
                    {
                        OnFlavourText(this, new FlavourTextEventArgs { name = Name, message = "Loading torpedos.", team = Team , level = FlavourTextEventArgs.LEVEL_INTERNAL});
                        foreach (TorpedoTube tube in unloadedTorpedoTubes)
                        {
                            tube.Load();
                        }
                        break;
                    }
                    OnFlavourText(this, new FlavourTextEventArgs { name = Name, message = "But we don't have any torpedos, Captain!", team = Team, level = FlavourTextEventArgs.LEVEL_INTERNAL });
                    break;
                case OrderType.SHEILD:
                    var status = (order as ShieldOrder).Status;
                    var undestroyedShields =Shields.Where(x => !x.IsDestroyed());
                    if (!undestroyedShields.Any())
                    {
                        OnFlavourText(this, new FlavourTextEventArgs { name = Name, message = "We don't have any working shields!", team = Team, level = FlavourTextEventArgs.LEVEL_INTERNAL });
                        return;
                    }
                    foreach (var shield in undestroyedShields)
                    {
                        shield.Status = status;
                    }
                    if (status == ShieldStatus.LOWERED)
                    {
                        OnFlavourText(this, new FlavourTextEventArgs { name = Name, message = "Lowering Shields.", team = Team, level = FlavourTextEventArgs.LEVEL_INTERNAL });

                    }
                    else
                    {
                        OnFlavourText(this, new FlavourTextEventArgs { name = Name, message = "Raising Shields.", team = Team, level = FlavourTextEventArgs.LEVEL_INTERNAL });
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
