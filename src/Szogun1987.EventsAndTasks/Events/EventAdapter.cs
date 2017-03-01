using System;
using System.Threading.Tasks;

namespace Szogun1987.EventsAndTasks.Events
{
    public class EventAdapter : ITaskAdapter
    {
        private readonly EventApi _eventApi;

        private readonly EventToTask<ResultEventArgs> _getNextIntEventToTask;

        public EventAdapter(EventApi eventApi)
        {
            _eventApi = eventApi;

            _getNextIntEventToTask = new EventToTask<ResultEventArgs>(
                handler => _eventApi.GetNextIntCompleted += handler,
                handler => _eventApi.GetNextIntCompleted -= handler,
                handler => _eventApi.GetNextIntFailed += handler,
                handler => _eventApi.GetNextIntFailed -= handler,
                () => _eventApi.GetNextInt());
        }
        
        public async Task<int> GetNextInt()
        {
            var args = await _getNextIntEventToTask.Invoke();
            return args.Result;
        }

        public async Task<int> GetNextIntWithArg(string arg)
        {
            var eventToTask = new EventToTask<ResultEventArgs>(
                handler => _eventApi.GetNextIntWithArgCompleted += handler,
                handler => _eventApi.GetNextIntWithArgCompleted -= handler,
                handler => _eventApi.GetNextIntWithArgFailed += handler,
                handler => _eventApi.GetNextIntWithArgFailed -= handler,
                () => _eventApi.GetNextIntWithArg(arg));

            var eventArgs = await eventToTask.Invoke();
            return eventArgs.Result;
        }
    }
}