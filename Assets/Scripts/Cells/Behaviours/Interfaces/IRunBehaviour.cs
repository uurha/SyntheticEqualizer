using Unity.Collections;

namespace Cells.Behaviours.Interfaces
{
    public interface IRunBehaviour
    {
        public void RunBehaviour(NativeArray<BehaviourData> data);
    }
}
