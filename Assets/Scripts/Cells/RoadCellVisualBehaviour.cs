using System;
using Base;
using BehaviourSystem;
using BehaviourSystem.Default;
using BehaviourSystem.Interfaces;
using Cells.Items;
using CorePlugin.Attributes.Headers;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

namespace Cells
{
    public class RoadCellVisualBehaviour : MonoBehaviour, ICellVisualBehaviour
    {
        [ReferencesHeader]
        [SerializeField] private CellItem[] leftItems;
        [SerializeField] private CellItem[] rightItems;
        
        private TransitStruct _leftTransitData;
        private TransitStruct _rightTransitData;
        private JobHandle _handleLeft;
        private JobHandle _handleRight;

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
            if (!(_handleLeft.IsCompleted && _handleRight.IsCompleted))
                return;
            var bufferLeftData = new BehaviourData(_leftTransitData.InitialOrientations, data, Allocator.TempJob);
            var bufferRightData = new BehaviourData(_rightTransitData.InitialOrientations, data, Allocator.TempJob);
            _handleLeft = JobsExtensions.JobHandleConverter(_leftTransitData.UpdateData(bufferLeftData));
            _handleRight = JobsExtensions.JobHandleConverter(_rightTransitData.UpdateData(bufferRightData));

            _handleLeft.Complete();
            _handleRight.Complete();
        }

        private void OnDisable()
        {
            _leftTransitData.Dispose();
            _rightTransitData.Dispose();
        }
    }
}
