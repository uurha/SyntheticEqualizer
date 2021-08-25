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
        [ReadOnly] [DeallocateOnJobCompletion] private NativeArray<Orientation> _initialData;
        [ReadOnly] [DeallocateOnJobCompletion] private NativeArray<Orientation> _additionalData;

        private int _additionalDataLenght;
        private bool _isCreated;

        public NativeArray<Orientation> InitialData => _initialData;

        public NativeArray<Orientation> AdditionalData => _additionalData;

        public bool IsCreated => _isCreated;

        public int Lenght => _additionalDataLenght;

        public BehaviourData(Orientation[] initialData, Orientation[] additionalData, Allocator allocator)
        {
            _initialData = new NativeArray<Orientation>(initialData, allocator);
            _additionalData = new NativeArray<Orientation>(additionalData, allocator);
            _additionalDataLenght = additionalData.Length;
            _isCreated = _initialData.IsCreated || _additionalData.IsCreated;
        }

        public void Dispose()
        {
            if (_initialData.Length > 0 && _initialData.IsCreated) _initialData.Dispose();
            if (_additionalData.Length > 0 && _additionalData.IsCreated) _additionalData.Dispose();
            _isCreated = false;
        }
    }
}
