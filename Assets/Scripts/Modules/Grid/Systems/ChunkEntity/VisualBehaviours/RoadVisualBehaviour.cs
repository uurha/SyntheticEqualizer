using System;
using System.Threading.Tasks;
using Base;
using Base.BehaviourModel;
using Base.BehaviourModel.Interfaces;
using CorePlugin.Attributes.EditorAddons;
using CorePlugin.Attributes.EditorAddons.SelectAttributes;
using CorePlugin.Attributes.Headers;
using Extensions;
using Modules.GlobalSettings.Model;
using Modules.Grid.Model;
using Modules.Grid.Systems.ChunkEntity.Behaviours;
using Modules.Grid.Systems.ChunkEntity.Unit;
using Unity.Jobs;
using UnityEngine;

namespace Modules.Grid.Systems.ChunkEntity.VisualBehaviours
{
    [Serializable]
    public class RoadVisualBehaviour : IChunkVisual
    {
        [ReferencesHeader]
        [SerializeField] private ChunkUnit[] leftItems;

        [SerializeField] private ChunkUnit[] rightItems;
        
        [SelectImplementation(typeof(IOrientationOffsetParams))] [SerializeReference]
        private IOrientationOffsetParams orientationOffsetParams;

        private JobHandle _handleLeft;
        private JobHandle _handleRight;

        private TransitStruct _leftTransitData;
        private TransitStruct _rightTransitData;
        private bool _isInitialized;

        public void OnDisable()
        {
            _leftTransitData.Dispose();
            _rightTransitData.Dispose();
        }

        public async Task SetBlockSettings(ChunkUnitsSettings settings)
        {
            if (_isInitialized) return;
            using var timer = FPSTimer.Create();
            foreach (var unit in leftItems)
            {
                unit.SetUnitData(settings.GetRandomData());
                await timer.AwaitTargetFPS();
            }

            foreach (var unit in rightItems)
            {
                unit.SetUnitData(settings.GetRandomData());
                await timer.AwaitTargetFPS();
            }
            _isInitialized = true;
            timer.StopTimer();
        }

        public IChunkVisual Initialize()
        {
            if (_isInitialized) return this;
            if (_leftTransitData.IsCreated) _leftTransitData.Dispose();
            _leftTransitData = new TransitStruct(leftItems, new DefaultMoveBehaviour(), orientationOffsetParams);
            if (_rightTransitData.IsCreated) _rightTransitData.Dispose();
            _rightTransitData = new TransitStruct(rightItems, new DefaultMoveBehaviour(), orientationOffsetParams);
            return this;
        }

        public IChunkVisual Initialize(IJobBehaviour jobBehaviour)
        {
            if (_isInitialized) return this;
            if (_leftTransitData.IsCreated) _leftTransitData.Dispose();
            _leftTransitData = new TransitStruct(leftItems, jobBehaviour, orientationOffsetParams);
            if (_rightTransitData.IsCreated) _rightTransitData.Dispose();
            _rightTransitData = new TransitStruct(rightItems, jobBehaviour, orientationOffsetParams);
            return this;
        }

        public void RunBehaviour(Orientation[] data)
        {
            if (!_isInitialized) return;
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
