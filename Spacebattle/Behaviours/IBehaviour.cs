﻿using Spacebattle.orders;

namespace Spacebattle.Behaviours
{
    public interface IBehaviour
    {
        Order GetNextOrder();
    }
}