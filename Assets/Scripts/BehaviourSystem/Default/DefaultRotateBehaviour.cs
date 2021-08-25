using BehaviourSystem.Interfaces;
using Unity.Burst;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;

namespace BehaviourSystem.Default
{
    [BurstCompile]
    public struct DefaultRotateBehaviour : IJobBehaviour
    {
        [DeallocateOnJobCompletion]
        private BehaviourData _data;
        
        public void Execute(int index, TransformAccess transform)
        {
            transform.localRotation = _data.InitialData[index].Rotation * _data.AdditionalData[index % _data.Lenght].Rotation;
        }

        public void SetData(IBehaviourData data)
        {
            _data = (BehaviourData) data;
        }

        public void Dispose()
        {
            _data.Dispose();
        }
    }
}