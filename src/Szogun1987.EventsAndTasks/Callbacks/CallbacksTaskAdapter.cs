using System.Threading.Tasks;

namespace Szogun1987.EventsAndTasks.Callbacks
{
    public class CallbacksTaskAdapter : ITaskAdapter
    {
        private readonly CallbacksApi _callbacksApi;

        public CallbacksTaskAdapter(CallbacksApi callbacksApi)
        {
            _callbacksApi = callbacksApi;
        }

        public Task<int> GetNextInt()
        {
            return CallbacksHelper.Invoke<int>(_callbacksApi.GetNextInt);
        }

        public Task<int> GetNextIntWithArg(string arg)
        {
            return CallbacksHelper.Invoke<int>(callback => _callbacksApi.GetNextIntWithArg(arg, callback));
        }
    }
}