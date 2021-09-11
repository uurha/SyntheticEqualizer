using SubModules.Cell.Model;

namespace Base.BehaviourModel.Interfaces
{
    public interface ICellVisualBehaviour
    {
        public void RunBehaviour(Orientation[] data);

        public ICellVisualBehaviour Initialize();

        public ICellVisualBehaviour Initialize(IJobBehaviour jobBehaviour);
    }
}
