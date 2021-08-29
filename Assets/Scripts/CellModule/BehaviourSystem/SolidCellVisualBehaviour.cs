using Base;
using Base.BehaviourModel;
using Base.BehaviourModel.Interfaces;
using CellItemModule;
using CellItemModule.BehaviourSystem.Default;
using CorePlugin.Attributes.Headers;
using Unity.Jobs;
using UnityEngine;

namespace CellModule.BehaviourSystem
{
    public class SolidCellVisualBehaviour : MonoBehaviour, ICellVisualBehaviour
    {
        [ReferencesHeader]
        [SerializeField] private CellItem[] items;
        
        private TransitStruct _transitData;
        private JobHandle _handle;

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

        private void OnDisable()
        {
            _transitData.Dispose();
        }
    }
}
