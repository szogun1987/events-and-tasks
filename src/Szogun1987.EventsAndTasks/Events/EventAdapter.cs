using System;
using System.Threading.Tasks;

namespace Szogun1987.EventsAndTasks.Events
{
    public class EventAdapter : ITaskAdapter
    {
        private readonly EventApi _eventApi;

        private readonly EventToTask<EventApi, ResultEventArgs> _getNextIntEventToTask;

        public EventAdapter(EventApi eventApi)
        {
            _eventApi = eventApi;

            _getNextIntEventToTask = new EventToTask<EventApi, ResultEventArgs>(
                _eventApi,
                (context, handler) => context.GetNextIntCompleted += handler,
                (context, handler) => context.GetNextIntCompleted -= handler,
                (context, handler) => context.GetNextIntFailed += handler,
                (context, handler) => context.GetNextIntFailed -= handler,
                context => context.GetNextInt());
        }
        
        public async Task<int> GetNextInt()
        {
            var args = await _getNextIntEventToTask.Invoke();
            return args.Result;
        }

        public async Task<int> GetNextIntWithArg(string arg)
        {
            var eventToTask = new EventToTask<EventApi, ResultEventArgs>(
                _eventApi,
                (context, handler) => context.GetNextIntWithArgCompleted += handler,
                (context, handler) => context.GetNextIntWithArgCompleted -= handler,
                (context, handler) => context.GetNextIntWithArgFailed += handler,
                (context, handler) => context.GetNextIntWithArgFailed -= handler,
                context => context.GetNextIntWithArg(arg));

            var eventArgs = await eventToTask.Invoke();
            return eventArgs.Result;
        }
    }
}