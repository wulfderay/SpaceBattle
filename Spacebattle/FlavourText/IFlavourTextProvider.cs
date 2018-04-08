using System;

namespace Spacebattle.entity
{
    public interface IFlavourTextProvider
    {
        event EventHandler<FlavourTextEventArgs> FlavourTextEventHandler;
    }
}
