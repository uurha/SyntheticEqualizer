using System.Threading.Tasks;

namespace Base.BaseTypes.InitializationQueues.Interfaces
{
    public interface IInitializable
    {
        public Task Initialize();
        public Task Initialize(IInitializeParams initializeParams);
    }
}