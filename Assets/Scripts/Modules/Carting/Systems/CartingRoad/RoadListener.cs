using System;
using System.Collections.Generic;
using System.Linq;
using Base;
using Base.Deque;
using CorePlugin.Attributes.EditorAddons;
using CorePlugin.Cross.Events.Interface;
using CorePlugin.Extensions;
using Modules.Grid.Interfaces;
using Modules.Grid.Model;
using UnityEngine;

namespace Modules.Carting.Systems.CartingRoad
{
    [CoreManagerElement]
    public class RoadListener : MonoBehaviour, IEventSubscriber, IEventHandler
    {
        [SerializeField] private int roadLookAhead = 10;
        private Queue<ICellComponent> _currentRoad = new Queue<ICellComponent>();
        private event GridEvents.RequestNextGrid RequestNextGrid;
        private event RoadEvents.OnRoadReadyEvent OnRoadReady;
        private bool _isInitialized;

        private void OnGridChanged(Conveyor<GridConfiguration> configurations, bool isInitialized)
        {
            _isInitialized = isInitialized;
            OnRoadReady?.Invoke(_isInitialized);
            _currentRoad = _isInitialized ? new Queue<ICellComponent>(_currentRoad.Concat(configurations.Last.RoadEntities)) : new Queue<ICellComponent>();
        }

        private ICellComponent GetNextRoadEntity()
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
