
using System;

namespace Szogun1987.EventsAndTasks
{
    /// <summary>
    /// Allows triggering from tests
    /// </summary>
    public interface ICompletionTrigger
    {
        void Complete(int result);

        void Fail(Exception exeception);
    }
}
