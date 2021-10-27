using System;
using Base;
using Base.Deque;
using CorePlugin.Attributes.Headers;
using CorePlugin.Cross.Events.Interface;
using Extensions;
using Modules.Grid.Model;
using Modules.Grid.Systems.Initializer;
using UnityEngine;

namespace Modules.Grid.Systems
{
    [RequireComponent(typeof(GridInitializer))]
    public class GridCullingMask : MonoBehaviour, IEventSubscriber
    {
        [SerializeField] private Vector3 cullingEnd;

        [SettingsHeader]
        [SerializeField] private Vector3 cullingStart;

        private Conveyor<GridConfiguration> _configurations;
        private bool _isInitialized;
        
        private void Update()
        {
            if (_isInitialized) CheckMask();
        }

        private void CheckMask()
        {
            foreach (var gridConfiguration in _configurations)
                gridConfiguration.CalculateCellInBound(position => position.Between(cullingStart, cullingEnd));
        }

        private void OnGridChanged(Conveyor<GridConfiguration> configurations, bool isInitialized)
        {
            _configurations = configurations;
            _isInitialized = isInitialized;
        }

        public Delegate[] GetSubscribers()
        {
            return new Delegate[]
                   {
                       (GridEvents.GridConfigurationChangedEvent) OnGridChanged
                   };
        }
    }
}
