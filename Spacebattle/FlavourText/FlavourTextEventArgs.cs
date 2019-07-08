using System;

namespace Spacebattle.entity
{
     
    public class FlavourTextEventArgs :EventArgs
    {
        public const int LEVEL_DEBUG = 0;
        public const int LEVEL_INFO = 1;
        public const int LEVEL_INTERNAL = 2;

        public string name;
        public int team;
        public string message;
        public int level;
    }
}