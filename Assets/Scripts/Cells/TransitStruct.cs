using System.Linq;
using BehaviourSystem;
using BehaviourSystem.Interfaces;
using Cells.Items;
using UnityEngine.Jobs;

namespace Cells
{
    public struct TransitStruct
    {
        public IJobBehaviour JobBehaviour { get; }
        public Orientation[] InitialOrientations { get; }
        public TransformAccessArray AccessArray { get; }
        public bool IsCreated { get; }

        public TransitStruct(CellItem[] cellItems, IJobBehaviour behaviour)
        {
            JobBehaviour = behaviour;
            InitialOrientations = cellItems.Select(x => x.Initialize().InitialOrientation).ToArray();
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
        }
    }
}
