using System;
using System.Threading.Tasks;

namespace Szogun1987.EventsAndTasks.Callbacks
{
    public static class CallbacksHelper
    {
        public static Task<T> Invoke<T>(Action<Action<T, Exception>> asyncMethod)
        {
            var completionSource = new TaskCompletionSource<T>();
            asyncMethod((arg, exception) =>
            {
                if (exception != null)
                {
                    completionSource.SetException(exception);
                }
                else
                {
                    completionSource.SetResult(arg);
                }
            });
            return completionSource.Task;
        }
    }
}