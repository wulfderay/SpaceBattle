using System;
using Spacebattle.entity;
using Spacebattle.orders;

namespace Spacebattle.Orders
{
    class StatusOrder :Order
    {
        public StatusType StatusType;
        public Ship Ship { get; internal set; }
    }

    public enum StatusType
    {
        POWER = 0
    }
}
