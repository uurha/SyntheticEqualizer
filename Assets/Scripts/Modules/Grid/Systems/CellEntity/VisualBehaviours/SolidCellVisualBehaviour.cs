using Base.BehaviourModel;
using Base.BehaviourModel.Interfaces;
using CorePlugin.Attributes.Headers;
using Extensions;
using Modules.GlobalSettings.Model;
using Modules.Grid.Model;
using Modules.Grid.Systems.CellEntity.Behaviours;
using Modules.Grid.Systems.CellEntity.Unit;
using Unity.Jobs;
using UnityEngine;

namespace Modules.Grid.Systems.CellEntity.VisualBehaviours
{
    public class SolidCellVisualBehaviour : MonoBehaviour, ICellVisualBehaviour
    {
        [ReferencesHeader]
        [SerializeField] private CellUnit[] items;

        private JobHandle _handle;

        private TransitStruct _transitData;

        private void OnDisable()
        {
            _transitData.Dispose();
        }

        public ICellVisualBehaviour SetBlockSettings(CellUnitsSettings settings)
        {
            foreach (var cellUnit in items) cellUnit.SetCellUnitData(settings.GetRandomData());
            return this;
        }

        public ICellVisualBehaviour Initialize()
        {
            if (_transitData.IsCreated) _transitData.Dispose();
            _transitData = new TransitStruct(items, new DefaultMoveBehaviour());
            return this;
        }

        public ICellVisualBehaviour Initialize(IJobBehaviour jobBehaviour)
        {
            if (_transitData.IsCreated) _transitData.Dispose();
            _transitData = new TransitStruct(items, jobBehaviour);
            return this;
        }

        public void RunBehaviour(Orientation[] data)
        {
            if (!_handle.IsCompleted) return;
            var bufferData = new BehaviourData(_transitData.InitialOrientations, data);
            _handle = JobsExtensions.JobHandleConverter(_transitData.UpdateData(bufferData));
            _handle.Complete();
        }
    }
}
