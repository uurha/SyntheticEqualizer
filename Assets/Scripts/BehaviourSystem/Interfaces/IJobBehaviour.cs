using UnityEngine.Jobs;

namespace BehaviourSystem.Interfaces
{
    public interface IJobBehaviour : IJobParallelForTransform
    {
        public void SetData(IBehaviourData data);
    }
}
