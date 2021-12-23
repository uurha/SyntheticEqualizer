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
    public class SolidVisualBehaviour : IChunkVisual
    {
        [ReferencesHeader]
        [SerializeField] private ChunkUnit[] items;

        [SelectImplementation(typeof(IOrientationOffsetParams))] [SerializeReference]
        private IOrientationOffsetParams orientationOffsetParams;

        private JobHandle _handle;

        private TransitStruct _transitData;

        private bool _isInitialized;

        public void OnDisable()
        {
            _transitData.Dispose();
        }

        public async Task SetBlockSettings(ChunkUnitsSettings settings)
        {
            if (_isInitialized) return;
            using var timer = FPSTimer.Create();

            foreach (var unit in items)
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
            if (_transitData.IsCreated) _transitData.Dispose();
            _transitData = new TransitStruct(items, new DefaultMoveBehaviour(), orientationOffsetParams);
            return this;
        }

        public IChunkVisual Initialize(IJobBehaviour jobBehaviour)
        {
            if (_isInitialized) return this;
            if (_transitData.IsCreated) _transitData.Dispose();
            _transitData = new TransitStruct(items, jobBehaviour, orientationOffsetParams);
            return this;
        }

        public void RunBehaviour(Orientation[] data)
        {
            if (!_isInitialized) return;
            if (!_handle.IsCompleted) return;
            var bufferData = new BehaviourData(_transitData.InitialOrientations, data);
            _handle = JobsExtensions.JobHandleConverter(_transitData.UpdateData(bufferData));
            _handle.Complete();
        }
    }
}
