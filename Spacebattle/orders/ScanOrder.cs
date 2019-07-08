using Spacebattle.entity;

namespace Spacebattle.orders
{
    internal class ScanOrder:Order
    {
        public Ship ShipToScan { get; set; }
    }
}