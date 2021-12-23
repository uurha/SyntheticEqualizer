using Base.BehaviourModel;
using Base.BehaviourModel.Interfaces;
using Unity.Burst;
using UnityEngine.Jobs;

namespace Modules.Grid.Systems.ChunkEntity.Behaviours
{
    [BurstCompile]
    public struct DefaultScaleBehaviour : IJobBehaviour
    {
        private BehaviourData _data;

        public void Execute(int index, TransformAccess transform)
        {
            var position = _data.InitialData[index].Position;
            var additional = _data.AdditionalData[index % _data.AdditionalDataLenght].Position;

            transform.localScale = position + additional;
        }

        public void SetData(IBehaviourData data)
        {
            _data = (BehaviourData) data;
        }
    }
}
