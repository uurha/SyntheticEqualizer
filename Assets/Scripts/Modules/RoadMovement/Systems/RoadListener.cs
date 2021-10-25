using System;
using System.Collections.Generic;
using System.Linq;
using Base;
using Base.Deque;
using CorePlugin.Cross.Events.Interface;
using CorePlugin.Extensions;
using Modules.Grid.Model;
using SubModules.Cell.Interfaces;
using UnityEngine;

namespace Modules.RoadMovement.Systems
{
    public class RoadListener : MonoBehaviour, IEventSubscriber, IEventHandler
    {
        private Queue<ICellEntity> _currentRoad = new Queue<ICellEntity>();
        private event GridEvents.RequestNextGrid RequestNextGrid;

        private void OnGridChanged(Conveyor<GridConfiguration> configurations, bool isInitialized)
        {
            _currentRoad = new Queue<ICellEntity>(_currentRoad.Concat(configurations.Last.RoadEntities));
        }

        private ICellEntity GetNextRoadEntity()
        {
            if (_currentRoad.Count <= 1)
            {
                RequestNextGrid?.Invoke();
            }
            return _currentRoad.Dequeue();
        }
        
        public Delegate[] GetSubscribers()
        {
            return new Delegate[]
                   {
                       (GridEvents.GridConfigurationChangedEvent) OnGridChanged
                   };
        }

        public void InvokeEvents()
        {
            
        }

        public void Subscribe(params Delegate[] subscribers)
        {
            EventExtensions.Subscribe(ref RequestNextGrid, subscribers);
        }

        public void Unsubscribe(params Delegate[] unsubscribers)
        {
            EventExtensions.Unsubscribe(ref RequestNextGrid, unsubscribers);
        }
    }
}
