using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Szogun1987.EventsAndTasks.Callbacks.NeverCalledback
{
    public class WorkedAroundAdapter : ITaskAdapter
    {
        private readonly Func<CallbacksApi> _callbacksApiFactory;
        private readonly List<CallbacksApi> _callbacksApis;
        private object _callbacksApisLockRoot;

        public WorkedAroundAdapter(Func<CallbacksApi> callbacksApiFactory)
        {
            _callbacksApiFactory = callbacksApiFactory;
            _callbacksApis = new List<CallbacksApi>();
            _callbacksApisLockRoot = new object();
        }

        public Task<int> GetNextInt()
        {
            var api = _callbacksApiFactory();
            lock (_callbacksApisLockRoot)
            {
                _callbacksApis.Add(api);
            }

            var completionSource = new TaskCompletionSource<int>();
            api.GetNextInt((result, exception) =>
            {
                lock (_callbacksApisLockRoot)
                {
                    _callbacksApis.Remove(api);
                }
                if (exception != null)
                {
                    completionSource.SetException(exception);
                }
                else
                {
                    completionSource.SetResult(result);
                }
            });
            return completionSource.Task;
        }

        public Task<int> GetNextIntWithArg(string arg)
        {
            var api = _callbacksApiFactory();
            lock (_callbacksApisLockRoot)
            {
                _callbacksApis.Add(api);
            }

            var completionSource = new TaskCompletionSource<int>();
            api.GetNextIntWithArg(arg, (result, exception) =>
            {
                lock (_callbacksApisLockRoot)
                {
                    _callbacksApis.Remove(api);
                }
                if (exception != null)
                {
                    completionSource.SetException(exception);
                }
                else
                {
                    completionSource.SetResult(result);
                }
            });
            return completionSource.Task;
        }
    }
}
