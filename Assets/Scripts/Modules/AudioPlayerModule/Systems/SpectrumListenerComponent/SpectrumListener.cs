using System;
using System.Collections.Generic;
using Base;
using CorePlugin.Attributes.Headers;
using CorePlugin.Cross.Events.Interface;
using CorePlugin.Extensions;
using Modules.AudioAnalyse.Model;
using Modules.AudioPlayerModule.Systems.AudioPlayerComponent;
using Modules.GlobalAudioSettings.Systems;
using UnityEngine;

namespace Modules.AudioPlayerModule.Systems.SpectrumListenerComponent
{
    [RequireComponent(typeof(AudioSource), typeof(AudioPlayer))]
    public class SpectrumListener : MonoBehaviour, IEventHandler, IEventSubscriber
    {
        [ReferencesHeader]
        [SerializeField] private AudioPlayer audioPlayer;

        [SerializeField] private AudioSource audioSource;

        private List<float[]> _spectrum;
        private SpectrumListenerSettings _currentListenerSettings;

        private event DataProcessorsEvents.SpectrumListenerDataEvent OnSpectrumDataEvent;

        private void Update()
        {
            if (!audioPlayer.IsPlaying ||
                audioPlayer.IsMuted)
                return;
            if (!_currentListenerSettings.IsValid) return;
            var channels = audioSource.clip.channels;
            CheckSettings(channels);
            var spectrumListenerData = new List<float[]>();

            for (var channel = 0; channel < channels; channel++)
            {
                audioSource.GetSpectrumData(_spectrum[channel], channel, _currentListenerSettings.FFTWindow);
                spectrumListenerData.Add(_spectrum[channel]);
            }

            var listenerData = new SpectrumListenerOutput(audioSource.clip.frequency,
                                                          _currentListenerSettings.NumberOfSamples,
                                                          channels,
                                                          spectrumListenerData);
            OnSpectrumDataEvent?.Invoke(listenerData);
        }

        private void CheckSettings(int channels)
        {
            if (_spectrum != null &&
                _spectrum.Count == channels)
                return;
            _spectrum = new List<float[]>();

            for (var channel = 0; channel < channels; channel++)
                _spectrum.Add(new float[_currentListenerSettings.NumberOfSamples]);
        }

        private void OnSpectrumListenerSettingsChanged(SpectrumListenerSettings listenerSettings)
        {
            _currentListenerSettings = listenerSettings;
        }

        public void InvokeEvents()
        {
        }

        public void Subscribe(params Delegate[] subscribers)
        {
            EventExtensions.Subscribe(ref OnSpectrumDataEvent, subscribers);
        }

        public void Unsubscribe(params Delegate[] unsubscribers)
        {
            EventExtensions.Unsubscribe(ref OnSpectrumDataEvent, unsubscribers);
        }

        public Delegate[] GetSubscribers()
        {
            return new Delegate[]
                   {
                       (GlobalAudioSettingsEvents.OnSpectrumListenerSettingsEvent) OnSpectrumListenerSettingsChanged
                   };
        }
    }
}
