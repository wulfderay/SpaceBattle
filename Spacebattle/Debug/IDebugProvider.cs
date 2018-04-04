using System;

namespace Spacebattle
{
    public interface IDebugProvider
    {
        event EventHandler<DebugEventArgs> DebugEventHandler;
    }

    public class DebugEventArgs
    {
        public string From;
        public string Message;
    }
}