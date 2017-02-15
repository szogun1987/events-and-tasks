using System;
using System.Collections.Generic;

namespace Szogun1987.EventsAndTasks.Callbacks
{
    public class CallbacksApi : ICompletionTrigger
    {
        private List<Action<int, Exception>> _waitingActions;

        public CallbacksApi()
        {
            _waitingActions = new List<Action<int, Exception>>();
        }

        public void Complete(int result)
        {
            InvokeCallbacks(result, null);
        }

        public void Fail(Exception exeception)
        {
           InvokeCallbacks(default(int), exeception);
        }

        private void InvokeCallbacks(int result, Exception exception)
        {
            var actionsToComplete = _waitingActions;
            _waitingActions = new List<Action<int, Exception>>();
            foreach (var action in actionsToComplete)
            {
                action(result, exception);
            }
        }

        public void GetNextInt(Action<int, Exception> callback)
        {
            _waitingActions.Add(callback);
        }

        public void GetNextIntWithArg(string s, Action<int, Exception> callback)
        {
            _waitingActions.Add(callback);
        }
    }
}