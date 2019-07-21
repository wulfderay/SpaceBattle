using Spacebattle.Configuration.EntityFactories;
using Spacebattle.Configuration.Schema;
using Spacebattle.entity;
using System.Collections.Generic;
using System.Linq;

namespace Spacebattle.Configuration.EntityConstructors
{
    public static class ShipFactory
    {
        public static Ship Construct(ShipSchema schema)
        {
            List<entity.parts.Reactor> reactors = schema.Reactors.Select(reactorSchema => ReactorFactory.Construct(reactorSchema)).ToList();
            List<entity.parts.Shield> shields = schema.Shields.Select(shieldSchema => ShieldFactory.Construct(shieldSchema)).ToList();
            List<entity.parts.Weapon.IWeapon> weapons = schema.Weapons.Select(weaponSchema => WeaponFactory.Construct(weaponSchema)).ToList();
            List<entity.parts.Engine> engines = schema.Engines.Select(engineSchema => EngineFactory.Construct(engineSchema)).ToList();
            List<entity.parts.CrewDeck> crewdecks = schema.CrewDecks.Select(crewDeckSchema => CrewDeckFactory.Construct(crewDeckSchema)).ToList();
            return new Ship(schema.Name, reactors, shields, weapons, engines, crewdecks);
        }
    }
}
