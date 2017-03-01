using System;

namespace Szogun1987.EventsAndTasks.Events
{
    public class FailureEventArgs : EventArgs
    {
        public FailureEventArgs(Exception error)
        {
            Error = error;
        }

        public Exception Error { get; private set; }
    }
}