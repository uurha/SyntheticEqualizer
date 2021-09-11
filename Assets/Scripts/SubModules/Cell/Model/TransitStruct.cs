using System.Linq;
using Base.BehaviourModel;
using Base.BehaviourModel.Interfaces;
using Unity.Collections;
using UnityEngine.Jobs;

namespace SubModules.Cell.Model
{
    public struct TransitStruct
    {
        public IJobBehaviour JobBehaviour { get; }
        public NativeArray<Orientation> InitialOrientations { get; }
        public TransformAccessArray AccessArray { get; }
        public bool IsCreated { get; }

        public TransitStruct(CellItem.CellItem[] cellItems, IJobBehaviour behaviour)
        {
            JobBehaviour = behaviour;

            InitialOrientations =
                new NativeArray<Orientation>(cellItems.Select(x => x.Initialize().InitialOrientation).ToArray(),
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
