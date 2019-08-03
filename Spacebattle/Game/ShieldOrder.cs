using Spacebattle.entity.parts;
using Spacebattle.orders;
using System.Collections.Generic;

namespace Spacebattle.Game
{
    class ShieldOrder : Order
    {
        public ShieldStatus Status { get; set; }
        public List<int> WhichShields { get; set; }
    }
}
