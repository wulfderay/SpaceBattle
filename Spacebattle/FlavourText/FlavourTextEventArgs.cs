using System;

namespace Spacebattle.entity
{
    public class FlavourTextEventArgs :EventArgs
    {
        public string name;
        public int team;
        public string message;
    }
}