using System;
using Spacebattle.entity;

namespace Spacebattle.Game
{
    public class ViewEventArgs
    {
        internal static ScanEvent Scan(IShip shipToScan)
        {
            return new ScanEvent { Ship = shipToScan };
        }
    }
}