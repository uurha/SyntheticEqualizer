using Base.BehaviourModel.Interfaces;
using Modules.Grid.Model;
using Unity.Burst;
using Unity.Collections;

namespace Base.BehaviourModel
{
    [BurstCompile]
    public readonly struct BehaviourData : IBehaviourData
    {
        [field: ReadOnly]
        public NativeArray<Orientation> InitialData { get; }

        [field: ReadOnly]
        [field: DeallocateOnJobCompletion]
        public NativeArray<Orientation> AdditionalData { get; }

        public int Lenght { get; }

        public BehaviourData(NativeArray<Orientation> initialData, Orientation[] additionalData)
        {
            InitialData = initialData;
            AdditionalData = new NativeArray<Orientation>(additionalData, Allocator.TempJob);
            Lenght = additionalData.Length;
        }
    }
}
