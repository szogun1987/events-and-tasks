using System;
using System.Collections.Generic;

namespace Szogun1987.EventsAndTasks.Promises
{
    public class PromisesApi : ICompletionTrigger
    {
        private List<SamplePromise<int>> _futurePromises;

        public PromisesApi()
        {
            _futurePromises = new List<SamplePromise<int>>();
        }

        public void Complete(int result)
        {
            var promises = _futurePromises;
            _futurePromises = new List<SamplePromise<int>>();
            foreach (var samplePromise in promises)
            {
                samplePromise.FulFill(result);
            }
        }

        public void Fail(Exception exeception)
        {
            var promises = _futurePromises;
            _futurePromises = new List<SamplePromise<int>>();
            foreach (var samplePromise in promises)
            {
                samplePromise.Brake(exeception);
            }
        }

        public IPromise<int> GetNextInt()
        {
            var promise = new SamplePromise<int>();
            _futurePromises.Add(promise);
            return promise;
        }

        public IPromise<int> GetNextIntWithArg(string arg)
        {
            var promise = new SamplePromise<int>();
            _futurePromises.Add(promise);
            return promise;
        }
    }
}