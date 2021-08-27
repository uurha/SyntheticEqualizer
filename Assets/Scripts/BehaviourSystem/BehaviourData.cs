using BehaviourSystem.Interfaces;
using Cells;
using Unity.Burst;
using Unity.Collections;
using UnityEngine;

namespace BehaviourSystem
{
    [BurstCompile]
    public struct BehaviourData : IBehaviourData
    {
        [ReadOnly] private NativeArray<Orientation> _initialData;
        [ReadOnly] [DeallocateOnJobCompletion] private NativeArray<Orientation> _additionalData;

        private int _additionalDataLenght;
        private bool _isCreated;

        public NativeArray<Orientation> InitialData => _initialData;

        public NativeArray<Orientation> AdditionalData => _additionalData;

        public int Lenght => _additionalDataLenght;

        public BehaviourData(NativeArray<Orientation> initialData, Orientation[] additionalData)
        {
            _initialData = initialData;
            _additionalData = new NativeArray<Orientation>(additionalData, Allocator.TempJob);
            _additionalDataLenght = additionalData.Length;
            _isCreated = _initialData.IsCreated || _additionalData.IsCreated;
        }
    }
}
