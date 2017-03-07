using System.Threading.Tasks;
using Szogun1987.EventsAndTasks.Events;

namespace Szogun1987.EventsAndTasks.NonBuilding.Events
{
    public class EventAdapter : ITaskAdapter
    {
        private readonly EventApi _eventApi;

        private readonly EventToTask<EventApi, ResultEventArgs> _getNextIntEventToTask;

        public EventAdapter(EventApi eventApi)
        {
            _eventApi = eventApi;

            _getNextIntEventToTask = EventToTask
                .Create(_eventApi)
                .WithTrigger(context => context.GetNextInt())
                .WithResultEvent<ResultEventArgs>((context, handler) => context.GetNextIntCompleted += handler)
                .WithFailureEvent((context, handler) => context.GetNextIntFailed += handler)
                .Build();
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