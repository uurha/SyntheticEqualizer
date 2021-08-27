using System.Linq;
using BehaviourSystem;
using BehaviourSystem.Interfaces;
using Cells.Items;
using Unity.Collections;
using UnityEngine.Jobs;

namespace Cells
{
    public struct TransitStruct
    {
        public IJobBehaviour JobBehaviour { get; }
        public NativeArray<Orientation> InitialOrientations { get; }
        public TransformAccessArray AccessArray { get; }
        public bool IsCreated { get; }

        public TransitStruct(CellItem[] cellItems, IJobBehaviour behaviour)
        {
            JobBehaviour = behaviour;
            InitialOrientations = new NativeArray<Orientation>(cellItems.Select(x => x.Initialize().InitialOrientation).ToArray(), Allocator.Persistent);
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
