using System;
using System.Threading.Tasks;

namespace Szogun1987.EventsAndTasks.Events
{
    public class EventToTask<TResult>
    {
        private readonly Action<EventHandler<TResult>> _subscribe;
        private readonly Action<EventHandler<TResult>> _unsubscribe;
        private readonly Action<EventHandler<FailureEventArgs>> _subscribeFailure;
        private readonly Action<EventHandler<FailureEventArgs>> _unsubscribeFailure;
        private readonly Action _trigger;

        public EventToTask(
            Action<EventHandler<TResult>> subscribe,
            Action<EventHandler<TResult>> unsubscribe,
            Action<EventHandler<FailureEventArgs>> subscribeFailure,
            Action<EventHandler<FailureEventArgs>> unsubscribeFailure,
            Action trigger)
        {
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
                // ReSharper disable AccessToModifiedClosure
                _unsubscribe(successHandler);
                _unsubscribeFailure(failureHandler);
                // ReSharper restore AccessToModifiedClosure
                completionSource.TrySetResult(args);
            };

            failureHandler = (sender, args) =>
            {
                // ReSharper disable AccessToModifiedClosure
                _unsubscribe(successHandler);
                _unsubscribeFailure(failureHandler);
                // ReSharper restore AccessToModifiedClosure
                completionSource.TrySetException(args.Error);
            };

            _subscribe(successHandler);
            _subscribeFailure(failureHandler);

            _trigger();

            return completionSource.Task;
        }
    }
}