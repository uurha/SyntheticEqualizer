using Unity.Collections;
using UnityEngine.Jobs;

namespace Cells.Behaviours.Interfaces
{
    public interface IBehaviour : IJobParallelForTransform
    {
        public void SetData(NativeArray<BehaviourData> data);
    }
}
