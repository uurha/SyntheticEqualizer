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
using Modules.GlobalSettings.Model;
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
        private CellUnitsSettings _blockColorSettings;
        private bool _isInitialized;
        private int _arrayLenght;
        private float[] _curveArray;

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

            if (_arrayLenght != floats.Length)
            {
                _arrayLenght = floats.Length;
                _curveArray = curve.CurveToArray(_arrayLenght);
            }
            var orientations = new Orientation[_arrayLenght];

            for (var index = 0; index < _arrayLenght; index++)
                orientations[index] = Selector(floats[index] * _curveArray[index]);
            foreach (var behaviour in _cellVisualBehaviours) behaviour.RunBehaviour(orientations);
        }

        private void OnAudioAnalyzerSettingsChanged(SpectrumAnalyzerSettings analyzerSettings)
        {
            _analyzerSettings = analyzerSettings;
        }

        private void OnBlockColorsChanged(CellUnitsSettings blockColorSettings)
        {
            _blockColorSettings = blockColorSettings;
            if (_cellVisualBehaviours == null) return;

            foreach (var visualBehaviour in _cellVisualBehaviours) visualBehaviour.SetBlockSettings(blockColorSettings);
        }

        private void OnGridConfigurationChanged(Conveyor<GridConfiguration> newGridConfigurations, bool isInitialized)
        {
            _isInitialized = isInitialized;

            _cellVisualBehaviours = newGridConfigurations
               .SelectMany(x => x.RowConfiguration
                                 .SelectMany(y => y.GetCells()
                                                   .Select(z => z.VisualBehaviourComponent?.Initialize().SetBlockSettings(_blockColorSettings)))).ToArray();
        }

        private Orientation Selector(float y)
        {
            var position = new Vector3(0, y, 0);
            return new Orientation(position, true);
        }

        public Delegate[] GetSubscribers()
        {
            return new Delegate[]
                   {
                       (GridEvents.GridConfigurationChangedEvent)OnGridConfigurationChanged,
                       (AudioAnalyzerEvents.SpectrumAnalyzerDataEvent)OnAnalyzedDataReceived,
                       (GlobalSettingsEvents.OnSpectrumAnalyzerSettingsEvent)OnAudioAnalyzerSettingsChanged,
                       (GlobalSettingsEvents.OnBlockColorSettingsEvents)OnBlockColorsChanged
                   };
        }
    }
}
