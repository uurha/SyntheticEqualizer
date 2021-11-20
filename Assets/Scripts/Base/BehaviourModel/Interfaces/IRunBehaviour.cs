using Modules.GlobalSettings.Model;
using Modules.Grid.Model;

namespace Base.BehaviourModel.Interfaces
{
    public interface ICellVisualBehaviour
    {
        public void RunBehaviour(Orientation[] data);

        public ICellVisualBehaviour SetBlockSettings(CellUnitsSettings settings);

        public ICellVisualBehaviour Initialize();

        public ICellVisualBehaviour Initialize(IJobBehaviour jobBehaviour);
    }
}
