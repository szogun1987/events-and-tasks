using System;

namespace Szogun1987.EventsAndTasks.Events
{
    public class EventApi : ICompletionTrigger
    {
        public void Complete(int result)
        {
            var args = new ResultEventArgs(result);
            GetNextIntCompleted?.Invoke(this, args);
            GetNextIntWithArgCompleted?.Invoke(this, args);
        }

        public void Fail(Exception exeception)
        {
            var args = new FailureEventArgs(exeception);
            GetNextIntFailed?.Invoke(this, args);
            GetNextIntWithArgFailed?.Invoke(this, args);
        }

        public void GetNextInt()
        {
            
        }

        public event EventHandler<ResultEventArgs> GetNextIntCompleted;

        public event EventHandler<FailureEventArgs> GetNextIntFailed;

        public void GetNextIntWithArg(string arg)
        {
            
        }

        public event EventHandler<ResultEventArgs> GetNextIntWithArgCompleted;

        public event EventHandler<FailureEventArgs> GetNextIntWithArgFailed;

    }
}