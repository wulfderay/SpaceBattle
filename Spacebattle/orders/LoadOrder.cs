using Spacebattle.entity.parts.Weapon;
using Spacebattle.orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spacebattle.Orders
{
    public class LoadOrder: Order
    {
        public WeaponType? WeaponType { get; set; }
        public string WeaponName { get; set; }
    }
}
