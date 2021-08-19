using Cells.Behaviours.Interfaces;
using Unity.Burst;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;

namespace Cells.Behaviours.Default
{
    [BurstCompile]
    public struct DefaultRotateBehaviour : IBehaviour
    {
        private NativeArray<BehaviourData> _data;
        
        public void Execute(int index, TransformAccess transform)
        {
            transform.localRotation = Quaternion.Euler(_data[index]._data);
        }

        public void SetData(NativeArray<BehaviourData> data)
        {
            _data = data;
        }
    }
}