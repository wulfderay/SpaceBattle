

using System;

namespace Spacebattle.entity
{
    interface IFlavourTextProvider
    {
        event EventHandler<FlavourTextEventArgs> FlavourTextEventHandler;
    }
}
