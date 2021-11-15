using Unity.Collections;
using UnityEngine.Jobs;

namespace Base.BehaviourModel.Interfaces
{
    public interface IJobBehaviour : IJobParallelForTransform
    {
        public void SetData(IBehaviourData data);
    }
}
