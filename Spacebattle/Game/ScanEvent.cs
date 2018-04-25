using Spacebattle.entity;

namespace Spacebattle.Game
{
    internal class ScanEvent :ViewEventArgs
    {
        public Ship Ship { get; set; }
    }
}