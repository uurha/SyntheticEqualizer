using System;
using System.Collections.Generic;
using System.Linq;
using Base;
using Base.Deque;
using BehaviourSystem.Interfaces;
using Cells;
using CorePlugin.Cross.Events.Interface;
using Grid.Model;
using UnityEngine;

namespace AudioSystem.DataProcessor
{
    public class AudioDataProcessor : MonoBehaviour, IEventSubscriber
    {
        [SerializeField] private float valueMultiplier;
        private ICellVisualBehaviour[] _cellVisualBehaviours;
        
        private void OnGridDataUpdated(Conveyor<GridConfiguration> newGridConfigurations)
        {
            _cellVisualBehaviours = newGridConfigurations.SelectMany(x => x.RowConfiguration.SelectMany(y => y.GetCells().Select(z=>z.VisualBehaviour.Initialize())))
                                                  .ToArray();
        }

        private void OnMeanLevelsUpdated(float[] levels)
        {
            if (_cellVisualBehaviours == null) return;
            var orientations = levels.Select(y =>
                                             {
                                                 var position = new Vector3(0, y) * valueMultiplier;
                                                 return new Orientation(position);
                                             }).ToArray();

            foreach (var behaviour in _cellVisualBehaviours)
            {
                behaviour.RunBehaviour(orientations);
            }
        }


        public IEnumerable<Delegate> GetSubscribers()
        {
            return new Delegate[] {(CrossEventsType.OnGridChanged) OnGridDataUpdated, (CrossEventsType.AudioMeanLevelsUpdated)OnMeanLevelsUpdated};
        }
    }
}
