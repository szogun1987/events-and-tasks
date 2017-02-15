using System.Threading.Tasks;

namespace Szogun1987.EventsAndTasks.Promises
{
    public class PromisesAdapter: ITaskAdapter
    {
        private readonly PromisesApi _promisesApi;

        public PromisesAdapter(PromisesApi promisesApi)
        {
            _promisesApi = promisesApi;
        }

        public Task<int> GetNextInt()
        {
            return _promisesApi.GetNextInt().ToTask();
        }

        public Task<int> GetNextIntWithArg(string arg)
        {
            return _promisesApi.GetNextIntWithArg(arg).ToTask();
        }
    }
}