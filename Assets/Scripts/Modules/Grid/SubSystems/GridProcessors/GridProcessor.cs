using System;
using System.Linq;
using Base;
using Base.BehaviourModel.Interfaces;
using Base.Deque;
using CorePlugin.Cross.Events.Interface;
using Modules.AudioAnalyse.Model;
using Modules.Grid.Model;
using SubModules.Cell.Model;
using UnityEngine;

namespace Modules.Grid.SubSystems.GridProcessors
{
    public class GridProcessor : MonoBehaviour, IEventSubscriber
    {
        [SerializeField] private float valueMultiplier = 2f;
        private ICellVisualBehaviour[] _cellVisualBehaviours;

        private void OnAnalyzedDataUpdated(SpectrumAnalyzerData data)
        {
            if (_cellVisualBehaviours == null) return;

            var orientations = data.MeanSpectrumData[0].Select(y =>
                                                               {
                                                                   var position = new Vector3(0, y * valueMultiplier);
                                                                   return new Orientation(position);
                                                               }).ToArray();
            foreach (var behaviour in _cellVisualBehaviours) behaviour.RunBehaviour(orientations);
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
                       (GridEvents.GridUpdatedEvent) OnGridDataUpdated,
                       (AudioAnalyzerEvents.SpectrumAnalyzerDataEvent) OnAnalyzedDataUpdated
                   };
        }
    }
}
