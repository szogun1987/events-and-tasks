using System.Threading.Tasks;

namespace Szogun1987.EventsAndTasks
{
    public interface ITaskAdapter
    {
        Task<int> GetNextInt();

        Task<int> GetNextIntWithArg(string arg);
    }
}