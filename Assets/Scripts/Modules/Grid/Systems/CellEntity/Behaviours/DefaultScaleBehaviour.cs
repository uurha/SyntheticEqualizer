using Base.BehaviourModel;
using Base.BehaviourModel.Interfaces;
using Unity.Burst;
using UnityEngine.Jobs;

namespace Modules.Grid.Systems.CellEntity.Behaviours
{
    [BurstCompile]
    public struct DefaultScaleBehaviour : IJobBehaviour
    {
        private BehaviourData _data;

        public readonly void Execute(int index, TransformAccess transform)
        {
            transform.localScale =
                _data.InitialData[index].Position + _data.AdditionalData[index % _data.Lenght].Position;
        }

        public void SetData(IBehaviourData data)
        {
            _data = (BehaviourData) data;
        }
    }
}
