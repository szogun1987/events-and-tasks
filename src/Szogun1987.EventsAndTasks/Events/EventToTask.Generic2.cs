using System;
using System.Threading.Tasks;

namespace Szogun1987.EventsAndTasks.Events
{
    public class EventToTask<TContext, TResult>
    {
        private readonly TContext _context;
        private readonly Action<TContext, EventHandler<TResult>> _subscribe;
        private readonly Action<TContext, EventHandler<TResult>> _unsubscribe;
        private readonly Action<TContext, EventHandler<FailureEventArgs>> _subscribeFailure;
        private readonly Action<TContext, EventHandler<FailureEventArgs>> _unsubscribeFailure;
        private readonly Action<TContext> _trigger;

        public EventToTask(
            TContext context,
            Action<TContext, EventHandler<TResult>> subscribe,
            Action<TContext, EventHandler<TResult>> unsubscribe,
            Action<TContext, EventHandler<FailureEventArgs>> subscribeFailure,
            Action<TContext, EventHandler<FailureEventArgs>> unsubscribeFailure,
            Action<TContext> trigger)
        {
            _context = context;
            _subscribe = subscribe;
            _unsubscribe = unsubscribe;
            _subscribeFailure = subscribeFailure;
            _unsubscribeFailure = unsubscribeFailure;
            _trigger = trigger;
        }

        public Task<TResult> Invoke()
        {
            var completionSource = new TaskCompletionSource<TResult>();

            EventHandler<TResult> successHandler = null;
            EventHandler<FailureEventArgs> failureHandler = null;

            successHandler = (sender, args) =>
            {
                _unsubscribe(_context, successHandler);
                _unsubscribeFailure(_context, failureHandler);
                completionSource.TrySetResult(args);
            };

            failureHandler = (sender, args) =>
            {
                _unsubscribe(_context, successHandler);
                _unsubscribeFailure(_context, failureHandler);
                completionSource.TrySetException(args.Error);
            };

            _subscribe(_context, successHandler);
            _subscribeFailure(_context, failureHandler);

            _trigger(_context);

            return completionSource.Task;
        }
    }
}