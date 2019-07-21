using Spacebattle.Configuration.Schema;
using Spacebattle.entity.parts.Weapon;
using Spacebattle.Entity.parts.Weapon;
using Spacebattle.Visualizer;
using System.Collections.Generic;
using System.Linq;

namespace Spacebattle.Configuration.StandardConfigs
{
    public  static class Ships
    {


        public static ShipSchema SmallShip(string name)
        {
            return new ShipSchema()
            {
                Name = name,
                Reactors = new List<ReactorSchema>() { Reactors.SmallReactor(), Reactors.SmallReactor() },
                Shields = Shields.SurroundWithFastShields(2).Concat(Shields.SurroundWithFastShields(1)).ToList(),  // 2 layer shields.
                Weapons = new List<WeaponSchema>() { Weapons.StandardPlasmaBolt(), Weapons.StandardPlasmaBolt() },
                Engines = new List<EngineSchema>() { Engines.Thruster(), Engines.Thruster() },
                CrewDecks = new List<CrewDeckSchema>() { CrewDecks.Bridge(15) }
            };
        }
        public static ShipSchema LargeShip(string name)
        {
            return new ShipSchema()
            {
                Name = name,
                Reactors = new List<ReactorSchema> { Reactors.BigReactor(), Reactors.BigReactor() },
                Shields = Shields.SurroundWithBigShields(4),
                Weapons = new List<WeaponSchema>() {
                    new WeaponSchema()
                    {
                        WeaponType = WeaponType.PHASER,
                        Name = "Ph1",
                        MaxHealth = 150,
                        Mass = 1,
                        UpkeepCost = 10,
                        Power = 200,
                        Range = 500
                    },
                    new WeaponSchema()
                    {
                        WeaponType = WeaponType.PHASER,
                        Name = "Ph2",
                        MaxHealth = 150,
                        Mass = 1,
                        UpkeepCost = 10,
                        Power = 200,
                        Range = 500
                    },
                    new WeaponSchema()
                    {
                        WeaponType = WeaponType.PHASER,
                        Name = "Ph3",
                        MaxHealth = 150,
                        Mass = 1,
                        UpkeepCost = 10,
                        Power = 200,
                        Range = 500
                    }

                },
                Engines = new List<EngineSchema>() { Engines.MainSail() },
                CrewDecks = new List<CrewDeckSchema>()
                {
                    CrewDecks.Bridge(),
                    CrewDecks.EngineeringDeck(),
                    CrewDecks.MilitaryDeck(10),
                }

            };
        }

        public static ShipSchema Destroyer(string name)
        {
            return new ShipSchema()
            {
                Name = name,
                Reactors = new List<ReactorSchema>() { Reactors.SmallReactor(), Reactors.SmallReactor(), Reactors.SmallReactor() },
                Shields = Shields.SurroundWithFastShields(2).Concat(Shields.SurroundWithBigShields(1)).ToList(),
                Weapons = new List<WeaponSchema>() {
                    // Todo: add torpedo to standard weapons.
                    Weapons.StandardTorpedoTube(),
                    Weapons.StandardTorpedoTube(),
                    Weapons.StandardPlasmaBolt(),
                 },
                Engines = new List<EngineSchema>() { Engines.CoreDrive() },
                CrewDecks = new List<CrewDeckSchema>() { CrewDecks.EngineeringDeck(35), CrewDecks.Bridge() }
            };

        }



        public static ShipSchema Fighter(string name)
        {
            return new ShipSchema()
            {
                Name = name,
                Reactors = new List<ReactorSchema>() { Reactors.SmallReactor(), Reactors.SmallReactor() },
                Shields = Shields.SurroundWithBigShields(2),
                Weapons = new List<WeaponSchema>() { Weapons.StandardPlasmaBolt() },
                Engines = new List<EngineSchema>() { new EngineSchema("Engine", 100, 20, 50, 100) },
                CrewDecks = new List<CrewDeckSchema>() { CrewDecks.Bridge(3) }
            };

        }
    }
}
