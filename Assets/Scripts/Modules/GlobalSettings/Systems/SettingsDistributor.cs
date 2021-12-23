using System;
using Base;
using CorePlugin.Attributes.EditorAddons;
using CorePlugin.Attributes.Headers;
using CorePlugin.Attributes.Validation;
using CorePlugin.Cross.Events.Interface;
using CorePlugin.Extensions;
using Extensions;
using Modules.AudioAnalyse.Systems.SpectrumAnalyse;
using Modules.AudioPlayer.Systems.SpectrumListenerComponent;
using Modules.GlobalSettings.Model;
using Modules.GlobalSettings.Presets;
using UnityEngine;

namespace Modules.GlobalSettings.Systems
{
    [CoreManagerElement]
    public class SettingsDistributor : MonoBehaviour, IEventHandler
    {
        [SettingsHeader(nameof(SpectrumListener))]
        [SerializeField] private FFTWindow fftWindow = FFTWindow.BlackmanHarris;

        [SerializeField] private int numberOfSamples = 1024;

        [SettingsHeader(nameof(SpectrumAnalyzer))]
        [SerializeField] private AudioExtensions.BandType bandType = AudioExtensions.BandType.ThirtyOneBand;

        [SerializeField] private float fallSpeed = 0.08f;
        [SerializeField] private float sensibility = 8.0f;
        
        [SettingsHeader(nameof(BlockColorPreset))]
        [NotNull][SerializeField] private BlockColorPreset blockColorsPreset;

        private SpectrumAnalyzerSettings _currentAnalyzerSettings;
        private SpectrumListenerSettings _currentListenerSettings;
        private ChunkUnitsSettings _currentBlockColorsSettings;

        private event GlobalSettingsEvents.OnSpectrumAnalyzerSettingsEvent OnSpectrumAnalyzerSettingsChanged;
        private event GlobalSettingsEvents.OnSpectrumListenerSettingsEvent OnSpectrumListenerSettingsChanged;
        private event GlobalSettingsEvents.OnBlockColorSettingsEvents OnBlockColorSettingsChanged;

        private void Awake()
        {
            GenerateAndStoreSetting();
        }

        private void GenerateAndStoreSetting()
        {
            _currentAnalyzerSettings = new SpectrumAnalyzerSettings(bandType, sensibility, fallSpeed);
            _currentListenerSettings = new SpectrumListenerSettings(fftWindow, numberOfSamples);
            _currentBlockColorsSettings = blockColorsPreset.GetSettings();
        }

        #if UNITY_EDITOR
        public void OnValidate()
        {
            if (!Application.isEditor ||
                !Application.isPlaying)
                return;
            GenerateAndStoreSetting();
            InvokeEvents();
        }
        #endif

        public void InvokeEvents()
        {
            OnSpectrumAnalyzerSettingsChanged?.Invoke(_currentAnalyzerSettings);
            OnSpectrumListenerSettingsChanged?.Invoke(_currentListenerSettings);
            OnBlockColorSettingsChanged?.Invoke(_currentBlockColorsSettings);
        }

        public void Subscribe(params Delegate[] subscribers)
        {
            EventExtensions.Subscribe(ref OnSpectrumAnalyzerSettingsChanged, subscribers);
            EventExtensions.Subscribe(ref OnSpectrumListenerSettingsChanged, subscribers);
            EventExtensions.Subscribe(ref OnBlockColorSettingsChanged, subscribers);
        }

        public void Unsubscribe(params Delegate[] unsubscribers)
        {
            EventExtensions.Subscribe(ref OnSpectrumAnalyzerSettingsChanged, unsubscribers);
            EventExtensions.Subscribe(ref OnSpectrumListenerSettingsChanged, unsubscribers);
            EventExtensions.Subscribe(ref OnBlockColorSettingsChanged, unsubscribers);
        }
    }
}
