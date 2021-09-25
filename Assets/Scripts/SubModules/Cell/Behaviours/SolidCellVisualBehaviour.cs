using Base.BehaviourModel;
using Base.BehaviourModel.Interfaces;
using CorePlugin.Attributes.Headers;
using Extensions;
using SubModules.Cell.Model;
using SubModules.CellItem.Behaviours;
using Unity.Jobs;
using UnityEngine;

namespace SubModules.Cell.Behaviours
{
    public class SolidCellVisualBehaviour : MonoBehaviour, ICellVisualBehaviour
    {
        [ReferencesHeader]
        [SerializeField] private CellItem.CellItem[] items;

        private JobHandle _handle;

        private TransitStruct _transitData;

        private void OnDisable()
        {
            _transitData.Dispose();
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
