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
    public class RoadCellVisualBehaviour : MonoBehaviour, ICellVisualBehaviour
    {
        [ReferencesHeader]
        [SerializeField] private CellItem.CellItem[] leftItems;

        [SerializeField] private CellItem.CellItem[] rightItems;
        private JobHandle _handleLeft;
        private JobHandle _handleRight;

        private TransitStruct _leftTransitData;
        private TransitStruct _rightTransitData;

        private void OnDisable()
        {
            _leftTransitData.Dispose();
            _rightTransitData.Dispose();
        }

        public ICellVisualBehaviour Initialize()
        {
            if (_leftTransitData.IsCreated) _leftTransitData.Dispose();
            _leftTransitData = new TransitStruct(leftItems, new DefaultMoveBehaviour());
            if (_rightTransitData.IsCreated) _rightTransitData.Dispose();
            _rightTransitData = new TransitStruct(rightItems, new DefaultMoveBehaviour());
            return this;
        }

        public ICellVisualBehaviour Initialize(IJobBehaviour jobBehaviour)
        {
            if (_leftTransitData.IsCreated) _leftTransitData.Dispose();
            _leftTransitData = new TransitStruct(leftItems, jobBehaviour);
            if (_rightTransitData.IsCreated) _rightTransitData.Dispose();
            _rightTransitData = new TransitStruct(rightItems, jobBehaviour);
            return this;
        }

        public void RunBehaviour(Orientation[] data)
        {
            if (!(_handleLeft.IsCompleted && _handleRight.IsCompleted)) return;
            var bufferLeftData = new BehaviourData(_leftTransitData.InitialOrientations, data);
            var bufferRightData = new BehaviourData(_rightTransitData.InitialOrientations, data);
            _handleLeft = JobsExtensions.JobHandleConverter(_leftTransitData.UpdateData(bufferLeftData));
            _handleRight = JobsExtensions.JobHandleConverter(_rightTransitData.UpdateData(bufferRightData));
            _handleLeft.Complete();
            _handleRight.Complete();
        }
    }
}
