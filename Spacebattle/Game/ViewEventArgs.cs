using Spacebattle.entity;

namespace Spacebattle.Game
{
    public class ViewEventArgs
    {
        internal static ScanEvent Scan(Ship shipToScan)
        {
            return new ScanEvent { Ship = shipToScan };
        }
    }
}