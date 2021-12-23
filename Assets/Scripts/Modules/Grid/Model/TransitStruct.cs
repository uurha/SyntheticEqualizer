using System.Linq;
using Base.BehaviourModel;
using Base.BehaviourModel.Interfaces;
using Modules.Grid.Systems.ChunkEntity.Unit;
using Unity.Collections;
using UnityEngine.Jobs;

namespace Modules.Grid.Model
{
    public readonly struct TransitStruct
    {
        public IJobBehaviour JobBehaviour { get; }
        public NativeArray<Orientation> InitialOrientations { get; }
        public TransformAccessArray AccessArray { get; }
        public bool IsCreated { get; }

        public TransitStruct(ChunkUnit[] cellItems, IJobBehaviour behaviour, IOrientationOffsetParams offsetParams)
        {
            JobBehaviour = behaviour;

            InitialOrientations =
                new NativeArray<Orientation>(cellItems.Select(x => x.Initialize(offsetParams).InitialOrientation).ToArray(),
                                             Allocator.Persistent);
            AccessArray = new TransformAccessArray(cellItems.Select(x => x.transform).ToArray());
            IsCreated = true;
        }

        public TransitStruct UpdateData(BehaviourData behaviourData)
        {
            JobBehaviour.SetData(behaviourData);
            return this;
        }

        public void Dispose()
        {
            if (AccessArray.isCreated) AccessArray.Dispose();
            if (InitialOrientations.IsCreated) InitialOrientations.Dispose();
        }
    }
}
