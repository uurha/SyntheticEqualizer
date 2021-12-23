using System.Threading.Tasks;
using Modules.GlobalSettings.Model;
using Modules.Grid.Model;

namespace Base.BehaviourModel.Interfaces
{
    public interface IChunkVisual
    {
        public void RunBehaviour(Orientation[] data);

        public Task SetBlockSettings(ChunkUnitsSettings settings);

        public IChunkVisual Initialize();

        public IChunkVisual Initialize(IJobBehaviour jobBehaviour);

        public void OnDisable();
    }
}
