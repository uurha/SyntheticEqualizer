using System;
using System.Diagnostics;
using System.Linq;
using Base;
using Base.BehaviourModel.Interfaces;
using Base.Deque;
using CorePlugin.Attributes.EditorAddons;
using CorePlugin.Attributes.Headers;
using CorePlugin.Cross.Events.Interface;
using CorePlugin.Extensions;
using Extensions;
using Modules.AudioAnalyse.Model;
using Modules.GlobalAudioSettings.Systems;
using Modules.Grid.Model;
using SubModules.Cell.Model;
using UnityEngine;

namespace Modules.Grid.SubSystems.Processors
{
    [CoreManagerElement]
    public class GridProcessor : MonoBehaviour, IEventSubscriber
    {
        [SettingsHeader]
        [SerializeField] private AnimationCurve curve;

        private ICellVisualBehaviour[] _cellVisualBehaviours;
        private SpectrumAnalyzerSettings _analyzerSettings;

        [Conditional(EditorDefinition.UnityEditor)]
        [EditorButton]
        private void InitializeCurve()
        {
            curve.Clear();
            for (var i = 0f; i <= 1f; i += 0.1f) curve.AddKey(i, 1);
        }

        private void OnAnalyzedDataReceived(SpectrumAnalyzerOutput data)
        {
            if (_cellVisualBehaviours == null) return;
            if (!_analyzerSettings.IsValid) return;
            var arrayLenght = data.MeanSpectrumData[0].Length;
            var curveArray = curve.CurveToArray(arrayLenght);
            var orientations = new Orientation[arrayLenght];

            for (var index = 0; index < arrayLenght; index++)
                orientations[index] = Selector(data.MeanSpectrumData[0][index] * curveArray[index]);
            foreach (var behaviour in _cellVisualBehaviours) behaviour.RunBehaviour(orientations);
        }

        private void OnAudioAnalyzerSettingsChanged(SpectrumAnalyzerSettings analyzerSettings)
        {
            _analyzerSettings = analyzerSettings;
        }

        private void OnGridConfigurationChanged(Conveyor<GridConfiguration> newGridConfigurations)
        {
            _cellVisualBehaviours = newGridConfigurations
                                   .SelectMany(x => x.RowConfiguration.SelectMany(y => y.GetCells()
                                                  .Select(z => z.VisualBehaviour.Initialize())))
                                   .ToArray();
        }

        private Orientation Selector(float y)
        {
            var position = new Vector3(0, y);
            return new Orientation(position);
        }

        public Delegate[] GetSubscribers()
        {
            return new Delegate[]
                   {
                       (GridEvents.GridConfigurationChangedEvent) OnGridConfigurationChanged,
                       (AudioAnalyzerEvents.SpectrumAnalyzerDataEvent) OnAnalyzedDataReceived,
                       (GlobalAudioSettingsEvents.OnSpectrumAnalyzerSettingsEvent) OnAudioAnalyzerSettingsChanged
                   };
        }
    }
}
