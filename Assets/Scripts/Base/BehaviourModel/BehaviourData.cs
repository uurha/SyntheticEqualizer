using Base.BehaviourModel.Interfaces;
using CellModule;
using Unity.Burst;
using Unity.Collections;

namespace Base.BehaviourModel
{
    [BurstCompile]
    public struct BehaviourData : IBehaviourData
    {
        [ReadOnly] private NativeArray<Orientation> _initialData;
        [ReadOnly] [DeallocateOnJobCompletion] private NativeArray<Orientation> _additionalData;

        private int _additionalDataLenght;

        public NativeArray<Orientation> InitialData => _initialData;

        public NativeArray<Orientation> AdditionalData => _additionalData;

        public int Lenght => _additionalDataLenght;

        public BehaviourData(NativeArray<Orientation> initialData, Orientation[] additionalData)
        {
            _initialData = initialData;
            _additionalData = new NativeArray<Orientation>(additionalData, Allocator.TempJob);
            _additionalDataLenght = additionalData.Length;
        }
    }
}
