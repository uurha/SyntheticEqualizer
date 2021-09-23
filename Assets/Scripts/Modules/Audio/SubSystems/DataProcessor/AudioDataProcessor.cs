using System;
using System.Collections.Generic;
using System.Linq;
using Base;
using Base.BehaviourModel.Interfaces;
using Base.Deque;
using CorePlugin.Cross.Events.Interface;
using CorePlugin.Logger;
using Modules.Grid.Model;
using SubModules.Cell.Model;
using UnityEngine;

namespace Modules.Audio.SubSystems.DataProcessor
{
    public class AudioDataProcessor : MonoBehaviour, IEventSubscriber
    {
        [SerializeField] private float valueMultiplier;
        private ICellVisualBehaviour[] _cellVisualBehaviours;

        private void OnAnalyzedDataUpdated(List<float[]> data)
        {
            if (_cellVisualBehaviours == null) return;

            var orientations = data[0].Select(y =>
                                              {
                                                  var position = new Vector3(0, y) * valueMultiplier;
                                                  return new Orientation(position);
                                              }).ToArray();
            foreach (var behaviour in _cellVisualBehaviours) behaviour.RunBehaviour(orientations);
        }

        private void OnBeatDetected()
        {
            DebugLogger.Log("Beat");
        }

        private void OnBMPChanged(int bpm)
        {
            DebugLogger.Log($"{bpm} bpm");
        }

        private void OnGridDataUpdated(Conveyor<GridConfiguration> newGridConfigurations)
        {
            _cellVisualBehaviours = newGridConfigurations
                                   .SelectMany(x => x.RowConfiguration.SelectMany(y => y.GetCells()
                                                  .Select(z => z.VisualBehaviour.Initialize())))
                                   .ToArray();
        }

        public Delegate[] GetSubscribers()
        {
            return new Delegate[]
                   {
                       (CrossEvents.OnGridUpdatedEvent) OnGridDataUpdated,
                       (AudioPlayerEvents.OnAudioAnalyzedDataUpdateEvent) OnAnalyzedDataUpdated,
                       (BeatDetectionEvents.OnBeatDetectedEvent) OnBeatDetected
                   };
        }
    }
}
