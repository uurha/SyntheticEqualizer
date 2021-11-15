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
using Modules.AudioPlayerUI.Model;
using Modules.Grid.Model;
using UnityEngine;

namespace Modules.Grid.Systems.Processors
{
    [CoreManagerElement]
    public class GridProcessor : MonoBehaviour, IEventSubscriber
    {
        [SettingsHeader]
        [SerializeField] private AnimationCurve curve;

        private ICellVisualBehaviour[] _cellVisualBehaviours;
        private SpectrumAnalyzerSettings _analyzerSettings;
        private bool _isInitialized;

        [Conditional(EditorDefinition.UnityEditor)]
        [EditorButton] 
        private void InitializeCurve()
        {
            curve.Clear();
            for (var i = 0f; i <= 1f; i += 0.1f) curve.AddKey(i, 1);
        }

        private void OnAnalyzedDataReceived(SpectrumAnalyzerOutput data)
        {
            if (!_isInitialized) return;
            if (!_analyzerSettings.IsValid) return;
            var floats = data.MeanSpectrumData[0];
            var arrayLenght = floats.Length;
            var curveArray = curve.CurveToArray(arrayLenght);
            var orientations = new Orientation[arrayLenght];

            for (var index = 0; index < arrayLenght; index++)
                orientations[index] = Selector(floats[index] * curveArray[index]);
            foreach (var behaviour in _cellVisualBehaviours) behaviour.RunBehaviour(orientations);
        }

        private void OnAudioAnalyzerSettingsChanged(SpectrumAnalyzerSettings analyzerSettings)
        {
            _analyzerSettings = analyzerSettings;
        }

        private void OnGridConfigurationChanged(Conveyor<GridConfiguration> newGridConfigurations, bool isInitialized)
        {
            _isInitialized = isInitialized;

            _cellVisualBehaviours = newGridConfigurations
                                   .SelectMany(x => x.RowConfiguration.SelectMany(y => y.GetCells()
                                                  .Select(z => z.VisualBehaviourComponent?.Initialize())))
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
                       (GridEvents.GridConfigurationChangedEvent)OnGridConfigurationChanged,
                       (AudioAnalyzerEvents.SpectrumAnalyzerDataEvent)OnAnalyzedDataReceived,
                       (GlobalAudioSettingsEvents.OnSpectrumAnalyzerSettingsEvent)OnAudioAnalyzerSettingsChanged
                   };
        }
    }
}
