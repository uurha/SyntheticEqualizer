using Base.BehaviourModel;
using Base.BehaviourModel.Interfaces;
using Unity.Burst;
using UnityEngine.Jobs;

namespace CellItemModule.BehaviourSystem.Default
{
    [BurstCompile]
    public struct DefaultRotateBehaviour : IJobBehaviour
    {
        private BehaviourData _data;
        
        public void Execute(int index, TransformAccess transform)
        {
            transform.localRotation = _data.InitialData[index].Rotation * _data.AdditionalData[index % _data.Lenght].Rotation;
        }

        public void SetData(IBehaviourData data)
        {
            _data = (BehaviourData) data;
        }
    }
}