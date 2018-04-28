using Spacebattle.Entity;
using Spacebattle.Game;

namespace Spacebattle.entity
{
    public interface IShip : IControllableEntity, IFlavourTextProvider, IBehave
    {
        // Shaun reccommended that I make an IShip for id purposes.
        IGameState gameState { get; set; }
    }
}