using Spacebattle.entity;

namespace Spacebattle.Game
{
    internal class ScanEvent :ViewEventArgs
    {
        public IShip Ship { get; set; }
    }
}