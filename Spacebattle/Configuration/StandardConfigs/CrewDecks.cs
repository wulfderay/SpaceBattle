using Spacebattle.Configuration.Schema;

namespace Spacebattle.Configuration.StandardConfigs
{
    public class CrewDecks
    {

        public static CrewDeckSchema Bridge(uint crew = 10)
        {
            return new CrewDeckSchema("Bridge", 150, 50, 3, crew, 0.1f);
        }

        public static CrewDeckSchema MilitaryDeck(uint crew = 60)
        {
            return new CrewDeckSchema("Mil. Deck", 300, 200, 1, crew, 0.1f);
        }

        public static CrewDeckSchema PleasureDeck(uint crew = 500)
        {
            return new CrewDeckSchema("Pleas. Deck", 200, 400, 5, crew, 0.01f);
        }

        public static CrewDeckSchema EngineeringDeck(uint crew = 30)
        {
            return new CrewDeckSchema("Eng. Deck", 275, 200, 2, crew, 0.5f);
        }
    }
}
