using System;
using System.Collections.Generic;
using System.Linq;
using Base;
using Base.BehaviourModel.Interfaces;
using Base.Deque;
using CellModule;
using CorePlugin.Cross.Events.Interface;
using CorePlugin.Logger;
using GridModule.Model;
using UnityEngine;

namespace AudioModule.DataProcessor
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

        private void OnSpectrumUpdated(float[] levels)
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

        private void OnBeatDetected()
        {
            DebugLogger.Log("Beat");
        }

        private void OnBMPChanged(int bpm)
        {
            DebugLogger.Log($"{bpm} bpm");
        }

        public IEnumerable<Delegate> GetSubscribers()
        {
            return new Delegate[]
                   {
                       (CrossEventsType.OnGridUpdatedEvent) OnGridDataUpdated,
                       (CrossEventsType.OnSpectrumUpdatedEvent) OnSpectrumUpdated,
                       (CrossEventsType.OnBeatDetectedEvent) OnBeatDetected,
                       (CrossEventsType.OnBPMChangedEvent) OnBMPChanged
                   };
        }
    }
}
