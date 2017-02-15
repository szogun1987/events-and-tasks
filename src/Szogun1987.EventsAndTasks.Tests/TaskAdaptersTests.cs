using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Szogun1987.EventsAndTasks.Callbacks;
using Szogun1987.EventsAndTasks.Promises;
using Xunit;

namespace Szogun1987.EventsAndTasks.Tests
{
    public class TaskAdaptersTests
    {
        [Theory]
        [MemberData(nameof(TestDataSets))]
        public void If_OperationCompletes_ResultTaskCompletesAlso(ITaskAdapter adapter, ICompletionTrigger completionTrigger)
        {
            // Given
            const int expectedResult = 1;

            // When
            var task = adapter.GetNextInt();
            completionTrigger.Complete(expectedResult);
            
            // Then Pass
            Assert.True(task.IsCompleted);
            var result = task.Result;
            Assert.StrictEqual(expectedResult, result);
        }

        [Theory]
        [MemberData(nameof(TestDataSets))]
        public async Task If_OperationFails_ResultTaskFailsAlso(ITaskAdapter adapter, ICompletionTrigger completionTrigger)
        {
            // Given

            // When
            var task = adapter.GetNextInt();
            completionTrigger.Fail(new Exception());

            // Then Pass
            await Assert.ThrowsAsync<Exception>(() => task);
        }

        [Theory]
        [MemberData(nameof(TestDataSets))]
        public void If_ParametrizedOperationCompletes_ResultTaskCompletesAlso(ITaskAdapter adapter, ICompletionTrigger completionTrigger)
        {
            // Given
            const int expectedResult = 1;
            // When
            var task = adapter.GetNextIntWithArg("Bla");
            completionTrigger.Complete(expectedResult);

            // Then Pass
            Assert.True(task.IsCompleted);
            var result = task.Result;
            Assert.StrictEqual(expectedResult, result);
        }

        [Theory]
        [MemberData(nameof(TestDataSets))]
        public async Task If_ParametrizedOperationFails_ResultTaskFailsAlso(ITaskAdapter adapter, ICompletionTrigger completionTrigger)
        {
            // Given

            // When
            var task = adapter.GetNextIntWithArg("Bla");
            completionTrigger.Fail(new Exception());

            // Then Pass
            await Assert.ThrowsAsync<Exception>(() => task);
        }

        // XUnit requires it
        // ReSharper disable once MemberCanBePrivate.Global 
        public static IEnumerable<object[]> TestDataSets
        {
            get
            {
                {
                    var api = new CallbacksApi();
                    var adapter = new CallbacksTaskAdapter(api);
                    yield return new object[] { adapter, api };
                }
                {
                    var api = new PromisesApi();
                    var adapter = new PromisesAdapter(api);
                    yield return new object[] { adapter, api };
                }
            }
        }
    }
}
