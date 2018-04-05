using Spacebattle.entity.parts;

using System.Collections.Generic;
using System.Linq;
using System;
using Spacebattle.physics;

namespace Spacebattle.entity
{
    public class Ship : Entity, IDamageable, IUpdateable, IFlavourTextProvider
    {
        //TODO: make reactor power be used by other parts... have a power requirement for each other type that is refreshed each tick.
        private List<Reactor> _reactors;
        private List<Shield> _shields;
        private List<Weapon> _guns;
        private List<Engine> _engines;
        private List<CrewDeck> _crewDecks;
        private Ship _lockedShip;
        private string _name;

        private float _throttle;

        public event EventHandler<FlavourTextEventArgs> FlavourTextEventHandler;

        public float Throttle
        {
            get
            {
                return _throttle;
            }
            set
            {
                if (value < 0)
                    return;
                if (value > 100)
                    return;
                _throttle = value;
            }
        }

        public float Power { get; private set; }

        public List<CrewDeck> CrewDecks
        {
            get
            {
                return _crewDecks;
            }
        }

        public List<Engine> Engines
        {
            get
            {
                return _engines;
            }
        }

        public List<Weapon> Guns
        {
            get
            {
                return _guns;
            }
        }

        public List<Shield> Shields
        {
            get
            {
                return _shields;
            }
        }

        public List<Reactor> Reactors
        {
            get
            {
                return _reactors;
            }
        }

        public Ship LockedShip
        {
            get
            {
                return _lockedShip;
            }

            private set
            {
                _lockedShip = value;
            }
        }

        public Ship ( string name, List<Reactor> reactors, List<Shield> shields, List<Weapon> guns, List<Engine> engines, List<CrewDeck> crewDecks)
        {
            _name = name;
            _reactors = reactors;
            _shields = shields;
            _guns = guns;
            _engines = engines;
            _crewDecks = crewDecks;

            _reactors.ForEach(x => x.FlavourTextEventHandler += OnFlavourText);
            _shields.ForEach(x => x.FlavourTextEventHandler += OnFlavourText);
            _guns.ForEach(x => x.FlavourTextEventHandler += OnFlavourText);
            _engines.ForEach(x => x.FlavourTextEventHandler += OnFlavourText);
            _crewDecks.ForEach(x => x.FlavourTextEventHandler += OnFlavourText);

            Mass = getTotalMass();

            Position = new Vector2d();
            Velocity = new Vector2d();
            Orientation = 0;
        }


        public void Damage(float damage)
        {
            OnFlavourText( _name, _name+ " was Hit!");
            var residualDamage = damage;
            foreach (var shield in _shields)
            {
                if (residualDamage == 0)
                    break;
                residualDamage= shield.Absorb(damage);
            }
            OnFlavourText(_name, damage - residualDamage +" damage was absorbed by shields.");
            if (residualDamage == 0)
                return;
            OnFlavourText(_name, _name + " suffered a hit to the hull!!");
            switch (GameEngine.Random(5))
            {
                case 0:
                    OnFlavourText(_name, residualDamage + " damage to reactors.");
                    _reactors[GameEngine.Random(_reactors.Count)].Damage(residualDamage);
                    break;
                case 1:
                    if (_shields.Count == 0)
                        Damage(residualDamage);
                    OnFlavourText(_name, residualDamage + " damage to shields.");
                    _shields[GameEngine.Random(_shields.Count)].Damage(residualDamage);
                    break;
                case 2:
                    OnFlavourText(_name, residualDamage + " damage to weapons.");
                    _guns[GameEngine.Random(_guns.Count)].Damage(residualDamage);
                    break;
                case 3:
                    OnFlavourText(_name, residualDamage + " damage to engines.");
                    _engines[GameEngine.Random(_engines.Count)].Damage(residualDamage);
                    break;
                case 4:
                    OnFlavourText(_name, residualDamage + " damage to crew deck.");
                    _crewDecks[GameEngine.Random(_crewDecks.Count)].Damage(residualDamage);
                    break;
            }
        }

        public void AllStop()
        {
            Velocity = Vector2d.Zero;
            Throttle = 0;
            //TODO: get this done the correct way.
            /*
            var accelerationNeeded = Mass * Velocity.Magnitude();
            var maxAcceleration = GetMaxAcceleration();

            SetCourse(Velocity.DirectionInDegreesTo(new Vector2d { X = 1, Y = 0 }));
            if (maxAcceleration > accelerationNeeded)
                Throttle = 100 * (accelerationNeeded / maxAcceleration);
            else
                Throttle = 100;
                */
        }

        public bool IsDestroyed()
        {
            if (_crewDecks.Select(x => (int)x.GetCrew()).Sum() == 0)
                return true; // probably not exactly what we want. We will probably want a difference betweeen destroyed and derelict.
            foreach (var part in _reactors)
            {
                if (!part.IsDestroyed())
                    return false;
            }
            foreach (var part in _shields)
            {
                if (!part.IsDestroyed())
                    return false;
            }
            foreach (var part in _guns)
            {
                if (!part.IsDestroyed())
                    return false;
            }
            foreach (var part in _engines)
            {
                if (!part.IsDestroyed())
                    return false;
            }
            
            return true;
        }

        public void Update(uint roundNumber)
        {
            if (IsDestroyed())
                return;
            // this would be where we would do repairs, regen shields, move the ship etc.
            
            Power = _reactors.Select(x => x.Produce()).Sum();

            spendPower();

            moveShip();
            
            foreach (var crewDeck in _crewDecks)
            {
                repairARandomPartThatNeedsIt(crewDeck);
            }
        }

        

        public string GetName()
        {
            return _name;
        }

        private void spendPower()
        {
            // change to for loop so we have more control owver what gets power when and what happens if there isn't enough power.
            Power -=( _reactors.Select(x => x.GetUpkeepCost()).Sum() +
               _shields.Select(x => x.GetUpkeepCost()).Sum() +
               _guns.Select(x => x.GetUpkeepCost()).Sum() +
               _engines.Select(x => x.GetUpkeepCost()).Sum() +
               _crewDecks.Select(x => x.GetUpkeepCost()).Sum());
            if (Power < 0)
                Power = 0;
            foreach (var shield in _shields)
                Power = shield.Regen(Power);
        }

        public void LockOn(Ship ship)
        {
            if ( ship != this) // stop hitting yourself! Stop hitting yourself!
                LockedShip = ship;
        }

        public void ShootAt(Ship otherShip) // this isn't great because we should ideally be able to pick the gun we are shooting with.
        {
            foreach (var gun in _guns)
            {
                gun.Shoot(DistanceTo(otherShip), otherShip);
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
            var force = _engines.Select(x => x.getMaxForce()).Sum();
            var mass = getTotalMass();
            return force/mass;
        }

        /// <summary>
        ///  this is really only used for initialization. Use Entity.Mass for the ship's mass as it pertains to the physics model
        /// </summary>
        /// <returns>the mass of all combined shipParts</returns>
        private float getTotalMass()
        {
            return _reactors.Select(x=>x.GetMass()).Sum() +
                _shields.Select(x => x.GetMass()).Sum() +
                _guns.Select(x => x.GetMass()).Sum() +
                _engines.Select(x => x.GetMass()).Sum() +
                _crewDecks.Select(x => x.GetMass()).Sum();
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

        /// <summary>
        /// set a desired velocity relative to the center of the galaxy. 
        /// </summary>
        /// <param name="speed"></param>
        public void SetVelocity(float speed)
        {
            throw new NotImplementedException();
        }

        private void moveShip()
        {
            if (Throttle == 0)
                return;
            var enginePower = _engines.Select(x => x.Throttle(Throttle)).Sum();
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
            damagedParts.AddRange( _reactors.Where(x => x.GetHealth() < x.GetMaxHealth()));
            damagedParts.AddRange( _engines.Where(x => x.GetHealth() < x.GetMaxHealth()));
            damagedParts.AddRange(_guns.Where(x => x.GetHealth() < x.GetMaxHealth()));
            damagedParts.AddRange(_shields.Where(x => x.GetHealth() < x.GetMaxHealth()));
            damagedParts.AddRange(_crewDecks.Where(x => x.GetHealth() < x.GetMaxHealth()));

            if (damagedParts.Count() == 0)
                return;

            crewDeck.PerformRepair(damagedParts[GameEngine.Random(damagedParts.Count)]);
        }


        public override string ToString()
        {
            var result = "Ship:" + _name + 
                " Crew:" + _crewDecks.Select(x =>(int) x.GetCrew()).Sum()+ 
                " Mass: "+Mass+
                " Power:"+ Power + 
                " Position:[X:"+Position.X+" Y:"+Position.Y+" Heading:"+Orientation+ "° Velocity: X:"+Velocity.X+ " Y:"+ Velocity.Y +" Mag:"+ Velocity.Magnitude()+"]\n";
            result +=  string.Join(" ", _reactors.Select(x => x.ToString())) + "\n";
            result += string.Join(" ", _shields.Select(x => x.ToString())) + "\n";
            result += string.Join(" ", _guns.Select(x => x.ToString())) + "\n";
            result += string.Join(" ", _engines.Select(x => x.ToString())) + "Max :"+GetMaxAcceleration()+"\n";
            result +=  string.Join(" ", _crewDecks.Select(x => x.ToString())) + "\n";
            if (IsDestroyed())
                result += "(Destroyed)\n";
            return result;
        }

        private void OnFlavourText(string name, string message)
        {
            FlavourTextEventHandler?.Invoke(this, new FlavourTextEventArgs { name = name, message = message });
        }

        private void OnFlavourText(object sender, FlavourTextEventArgs e)
        {
            FlavourTextEventHandler?.Invoke(sender, new FlavourTextEventArgs { name = _name+"."+e.name, message = e.message });
        }
    }
}
