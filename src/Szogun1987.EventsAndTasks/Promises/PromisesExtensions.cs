using System.Threading.Tasks;

namespace Szogun1987.EventsAndTasks.Promises
{
    public static class PromisesExtensions
    {
        public static Task<T> ToTask<T>(this IPromise<T> promise)
        {
            var completionSource = new TaskCompletionSource<T>();

            promise
                .OnDone(completionSource.SetResult)
                .OnError(completionSource.SetException);

            return completionSource.Task;
        }
    }
}