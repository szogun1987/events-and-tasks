using System;
using System.Collections.Generic;
using System.Threading;
using Szogun1987.EventsAndTasks.Callbacks;
using Szogun1987.EventsAndTasks.Callbacks.NeverCalledback;
using Xunit;

namespace Szogun1987.EventsAndTasks.Tests
{
    public class NeverCalledBackTests
    {
        [Theory]
        [MemberData(nameof(TestDataSets))]
        public void OperationIsCompleted_EvenIf_GarbageCollector_IsTriggered(
            string name, 
            Func<Func<CallbacksApi>, ITaskAdapter> taskAdapterFactory)
        {
            // Given
            var apiReference = new WeakReference<CallbacksApi>(new CallbacksApi());
            var adapter = taskAdapterFactory(() =>
            {
                CallbacksApi api;
                apiReference.TryGetTarget(out api);
                return api;
            });

            // When
            var task = adapter.GetNextInt();
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
            CallbacksApi apiFromReference;
            if (apiReference.TryGetTarget(out apiFromReference))
            {
                apiFromReference.Complete(1);
            }
            else
            {
                Assert.True(false, $"API for {name} adapter evaporated");
            }

            // Then
            Assert.True(task.IsCompleted);

            // This line should prevent worked around adapter failure but it doesn't
            Assert.NotNull(adapter);

            // Ensure that API is collected when task is completed and it result is no longer used
            apiFromReference = null;
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
            Assert.False(apiReference.TryGetTarget(out apiFromReference));
        }
        
        public static IEnumerable<object[]> TestDataSets
        {
            get
            {
                yield return new object[] { "Broken", new Func<Func<CallbacksApi>, ITaskAdapter>(CreateBroken)};
                yield return new object[] { "Worked around", new Func<Func<CallbacksApi>, ITaskAdapter>(CreateWorkedAround) };
                yield return new object[] { "Fixed", new Func<Func<CallbacksApi>, ITaskAdapter>(CreateFixed) };
            }
        }

        private static BrokenTaskAdapter CreateBroken(Func<CallbacksApi> callbacksApiFactory)
        {
            return new BrokenTaskAdapter(callbacksApiFactory);
        }

        private static WorkedAroundAdapter CreateWorkedAround(Func<CallbacksApi> callbacksApiFactory)
        {
            return new WorkedAroundAdapter(callbacksApiFactory);
        }

        private static FixedTaskAdapter CreateFixed(Func<CallbacksApi> callbacksApiFactory)
        {
            return new FixedTaskAdapter(callbacksApiFactory);
        }
    }
}