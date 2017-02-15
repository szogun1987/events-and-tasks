using System;

namespace Szogun1987.EventsAndTasks.Promises
{
    public interface IPromise<T>
    {
        IPromise<T> OnDone(Action<T> action);

        IPromise<T> OnError(Action<Exception> action);

        IPromise<T> Always(Action action);
    }
}