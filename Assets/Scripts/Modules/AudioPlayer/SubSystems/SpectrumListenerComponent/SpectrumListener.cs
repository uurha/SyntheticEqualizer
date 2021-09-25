using System;
using System.Collections.Generic;
using Base;
using CorePlugin.Attributes.Headers;
using CorePlugin.Cross.Events.Interface;
using CorePlugin.Extensions;
using Modules.AudioAnalyse.Model;
using Modules.AudioPlayer.Model;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Modules.AudioPlayer.SubSystems.SpectrumListener
{
    [RequireComponent(typeof(AudioSource), typeof(AudioPlayer))]
    public class SpectrumListener : MonoBehaviour, IEventHandler
    {
        [SerializeField] private AudioPlayer audioPlayer;

        [ReferencesHeader]
        [SerializeField] private AudioSource audioSource;

        [SerializeField] private FFTWindow fftWindow = FFTWindow.BlackmanHarris;

        [SettingsHeader]
        [SerializeField] private int numberOfSamples = 1024;

        private List<float[]> _spectrum;

        private event DataProcessorsEvents.SpectrumListenerDataEvent OnSpectrumDataUpdated;

        private void Update()
        {
            if (!audioPlayer.IsPlaying) return;
            var channels = audioSource.clip.channels;
            CheckSettings(channels);

            var spectrumListenerData = new List<float[]>();
            for (var channel = 0; channel < channels; channel++)
            {
                audioSource.GetSpectrumData(_spectrum[channel], channel, fftWindow);
                spectrumListenerData.Add(_spectrum[channel]);
            }
            OnSpectrumDataUpdated?.Invoke(
                                          new SpectrumListenerData(audioSource.clip.frequency, numberOfSamples,
                                                                   channels,
                                                                   spectrumListenerData));
        }

        private void CheckSettings(int channels)
        {
            if (_spectrum != null &&
                _spectrum.Count == channels)
                return;
            _spectrum = new List<float[]>();
            for (var channel = 0; channel < channels; channel++) _spectrum.Add(new float[numberOfSamples]);
        }
        
        public void InvokeEvents()
        {
        }

        public void Subscribe(params Delegate[] subscribers)
        {
            EventExtensions.Subscribe(ref OnSpectrumDataUpdated, subscribers);
        }

        public void Unsubscribe(params Delegate[] unsubscribers)
        {
            EventExtensions.Unsubscribe(ref OnSpectrumDataUpdated, unsubscribers);
        }
    }
}
