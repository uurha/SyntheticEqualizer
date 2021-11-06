using System;
using System.Collections.Generic;
using System.Linq;
using Base;
using Base.Deque;
using CorePlugin.Attributes.EditorAddons;
using CorePlugin.Cross.Events.Interface;
using CorePlugin.Extensions;
using Modules.Carting.Interfaces;
using Modules.Grid.Model;
using UnityEngine;

namespace Modules.Carting.Systems.Road
{
    [CoreManagerElement]
    public class RoadListener : MonoBehaviour, IEventSubscriber, IEventHandler
    {
        [SerializeField] private int roadLookAhead = 10;
        private Queue<ICartingRoadComponent> _currentRoad = new Queue<ICartingRoadComponent>();
        
        private event GridEvents.RequestNextGrid RequestNextGrid;
        private event RoadEvents.OnRoadReadyEvent OnRoadReady;
        private bool _isInitialized;
        
        private void OnGridChanged(Conveyor<GridConfiguration> configurations, bool isInitialized)
        {
            _isInitialized = isInitialized;
            _currentRoad = _isInitialized ? new Queue<ICartingRoadComponent>(_currentRoad.Concat(configurations.Last.RoadEntities)) : new Queue<ICartingRoadComponent>();
            OnRoadReady?.Invoke(_isInitialized);
        }

        private ICartingRoadComponent GetNextRoadEntity()
        {
            if (!_isInitialized) return null;
            if (_currentRoad.Count <= roadLookAhead)
            {
                RequestNextGrid?.Invoke();
            }
            return _currentRoad.Dequeue();
        }

        public Delegate[] GetSubscribers()
        {
            return new Delegate[]
                   {
                       (GridEvents.GridConfigurationChangedEvent) OnGridChanged,
                       (RoadEvents.RequestNextRoadEntity) GetNextRoadEntity
                   };
        }

        public void InvokeEvents()
        {
        }

        public void Subscribe(params Delegate[] subscribers)
        {
            EventExtensions.Subscribe(ref RequestNextGrid, subscribers);
            EventExtensions.Subscribe(ref OnRoadReady, subscribers);
        }

        public void Unsubscribe(params Delegate[] unsubscribers)
        {
            EventExtensions.Unsubscribe(ref RequestNextGrid, unsubscribers);
            EventExtensions.Unsubscribe(ref OnRoadReady, unsubscribers);
        }
    }
}
