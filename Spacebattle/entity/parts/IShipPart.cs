﻿using Spacebattle.Game;
using System;

namespace Spacebattle.entity
{
    public interface IShipPart: IDamageable, IFlavourTextProvider, IGameEngineEventProvider, IUpdateable
    {
        Tuple<float, float> Health { get; } // current, max
        void Repair(float repairAmount);
        float GetMass();
        float GetUpkeepCost();
        string GetName();
        Ship Parent { get; set; }
    }
}