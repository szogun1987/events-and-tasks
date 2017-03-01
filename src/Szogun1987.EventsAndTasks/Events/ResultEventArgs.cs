using System;

namespace Szogun1987.EventsAndTasks.Events
{
    public class ResultEventArgs : EventArgs
    {
        public ResultEventArgs(int result)
        {
            Result = result;
        }

        public int Result { get; private set; }
    }
}
