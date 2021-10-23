using System;
using Base;
using CorePlugin.Attributes.EditorAddons;
using CorePlugin.Attributes.Headers;
using CorePlugin.Cross.Events.Interface;
using CorePlugin.Extensions;
using Extensions;
using Modules.AudioAnalyse.Systems.SpectrumAnalyse;
using Modules.AudioPlayerModule.Systems.SpectrumListenerComponent;
using Modules.AudioPlayerUI.Model;
using UnityEngine;

namespace Modules.GlobalAudioSettings.Systems
{
    [CoreManagerElement]
    public class AudioSettingsDistributor : MonoBehaviour, IEventHandler
    {
        [SettingsHeader(nameof(SpectrumListener))]
        [SerializeField] private FFTWindow fftWindow = FFTWindow.BlackmanHarris;

        [SerializeField] private int numberOfSamples = 1024;

        [SettingsHeader(nameof(SpectrumAnalyzer))]
        [SerializeField] private AudioExtensions.BandType bandType = AudioExtensions.BandType.ThirtyOneBand;

        [SerializeField] private float fallSpeed = 0.08f;
        [SerializeField] private float sensibility = 8.0f;

        private SpectrumAnalyzerSettings _currentAnalyzerSettings;
        private SpectrumListenerSettings _currentListenerSettings;

        private event GlobalAudioSettingsEvents.OnSpectrumAnalyzerSettingsEvent OnSpectrumAnalyzerSettingsChanged;
        private event GlobalAudioSettingsEvents.OnSpectrumListenerSettingsEvent OnSpectrumListenerSettingsChanged;

        private void Awake()
        {
            GenerateAndStoreSetting();
        }

        private void GenerateAndStoreSetting()
        {
            _currentAnalyzerSettings = new SpectrumAnalyzerSettings(bandType, sensibility, fallSpeed);
            _currentListenerSettings = new SpectrumListenerSettings(fftWindow, numberOfSamples);
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
        }

        public void Subscribe(params Delegate[] subscribers)
        {
            EventExtensions.Subscribe(ref OnSpectrumAnalyzerSettingsChanged, subscribers);
            EventExtensions.Subscribe(ref OnSpectrumListenerSettingsChanged, subscribers);
        }

        public void Unsubscribe(params Delegate[] unsubscribers)
        {
            EventExtensions.Subscribe(ref OnSpectrumAnalyzerSettingsChanged, unsubscribers);
            EventExtensions.Subscribe(ref OnSpectrumListenerSettingsChanged, unsubscribers);
        }
    }
}
