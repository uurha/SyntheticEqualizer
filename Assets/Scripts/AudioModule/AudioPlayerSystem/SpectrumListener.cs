using System;
using System.Collections.Generic;
using System.Linq;
using Base;
using CorePlugin.Attributes.Headers;
using CorePlugin.Cross.Events.Interface;
using UnityEngine;

namespace AudioModule.AudioPlayerSystem
{
    public struct SpectrumListenerData
    {
        public int Frequency { get; }
        public int NumberOfSamples { get; }
        public float[] SpectrumData { get; }

        public SpectrumListenerData(int frequency, int numberOfSamples, float[] spectrumData)
        {
            Frequency = frequency;
            NumberOfSamples = numberOfSamples;
            SpectrumData = spectrumData;
        }
    }
    
    [RequireComponent(typeof(AudioPlayer), typeof(AudioSource))]
    public class SpectrumListener : MonoBehaviour, IEventHandler, IEventSubscriber
    {
        [ReferencesHeader]
        [SerializeField] private AudioSource audioSource;
        
        [SettingsHeader]
        [SerializeField] private int numberOfSamples = 1024;
        [SerializeField] private int channel = 0;
        [SerializeField] private FFTWindow fftWindow = FFTWindow.BlackmanHarris;
        
        private event CrossEventsType.OnSpectrumListenerDataUpdateEvent OnSpectrumDataUpdated;

        private AudioPlayerState _currentState;
        private float[] _spectrum;

        private void Awake()
        {
            CheckSettings();
        }

        public void InvokeEvents()
        {

        }

        private void OnPlayerStateChanged(AudioPlayerState state)
        {
            _currentState = state switch
                            {
                                AudioPlayerState.Play => state,
                                AudioPlayerState.Stop => state,
                                _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
                            };
        }

        private void CheckSettings()
        {
            if (_spectrum == null ||
                _spectrum.Length != numberOfSamples)
            {
                _spectrum = new float[numberOfSamples];
            }
        }

        private void Update()
        {
            if (_currentState != AudioPlayerState.Play) return;
            CheckSettings();
            audioSource.GetSpectrumData(_spectrum, channel, fftWindow);

            var spectrumListenerData = new SpectrumListenerData(audioSource.clip.frequency, numberOfSamples ,_spectrum);
            OnSpectrumDataUpdated?.Invoke(spectrumListenerData);
        }

        public void Subscribe(IEnumerable<Delegate> subscribers)
        {
            foreach (var spectrumUpdateEvent in subscribers.OfType<CrossEventsType.OnSpectrumListenerDataUpdateEvent>())
            {
                OnSpectrumDataUpdated += spectrumUpdateEvent;
            }
        }

        public void Unsubscribe(IEnumerable<Delegate> unsubscribers)
        {
            foreach (var spectrumUpdateEvent in unsubscribers.OfType<CrossEventsType.OnSpectrumListenerDataUpdateEvent>())
            {
                OnSpectrumDataUpdated -= spectrumUpdateEvent;
            }
        }

        public IEnumerable<Delegate> GetSubscribers()
        {
            return new Delegate[] {(CrossEventsType.OnAudioPlayerStateEvent) OnPlayerStateChanged};
        }
    }
}
