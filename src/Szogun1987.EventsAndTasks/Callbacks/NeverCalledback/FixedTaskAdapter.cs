using System;
using System.Threading.Tasks;

namespace Szogun1987.EventsAndTasks.Callbacks.NeverCalledback
{
    public class FixedTaskAdapter : ITaskAdapter
    {
        private readonly Func<CallbacksApi> _callbacksApiFactory;

        public FixedTaskAdapter(Func<CallbacksApi> callbacksApiFactory)
        {
            _callbacksApiFactory = callbacksApiFactory;
        }

        public Task<int> GetNextInt()
        {
            var api = _callbacksApiFactory();
            var completionSource = new TaskCompletionSource<int>(api);
            api.GetNextInt((arg1, exception) =>
            {
                if (exception != null)
                {
                    completionSource.SetException(exception);
                }
                else
                {
                    completionSource.SetResult(arg1);
                }
            });
            return completionSource.Task;
        }

        public Task<int> GetNextIntWithArg(string arg)
        {
            var api = _callbacksApiFactory();
            var completionSource = new TaskCompletionSource<int>(api);
            api.GetNextIntWithArg(arg, (arg1, exception) =>
            {
                if (exception != null)
                {
                    completionSource.SetException(exception);
                }
                else
                {
                    completionSource.SetResult(arg1);
                }
            });
            return completionSource.Task;
        }
    }
}