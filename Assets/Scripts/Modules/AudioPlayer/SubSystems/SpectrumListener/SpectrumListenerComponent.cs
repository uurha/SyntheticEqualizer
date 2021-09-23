using System;
using System.Collections.Generic;
using Base;
using CorePlugin.Attributes.Headers;
using CorePlugin.Cross.Events.Interface;
using CorePlugin.Extensions;
using Modules.AudioPlayer.Model;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Modules.AudioPlayer.SubSystems.SpectrumListener
{
    [RequireComponent(typeof(AudioSource), typeof(AudioPlayerComponent))]
    public class SpectrumListenerComponent : MonoBehaviour, IEventHandler
    {
        [SerializeField] private AudioPlayerComponent audioPlayer;

        [ReferencesHeader]
        [SerializeField] private AudioSource audioSource;

        [SerializeField] private FFTWindow fftWindow = FFTWindow.BlackmanHarris;

        [SettingsHeader]
        [SerializeField] private int numberOfSamples = 1024;

        private JobHandle _handle;
        private List<float[]> _spectrum;

        private event CrossEvents.OnSpectrumListenerDataUpdateEvent OnSpectrumDataUpdated;

        private void Update()
        {
            if (!audioPlayer.IsPlaying) return;
            if (!_handle.IsCompleted) return;
            var channels = audioSource.clip.channels;
            CheckSettings(channels);

            var jobs = new MultiplyJob[channels];
            for (int channel = 0; channel < channels; channel++)
            {
                audioSource.GetSpectrumData(_spectrum[channel], channel, fftWindow);
                var volume = audioSource.volume;
                var multiplier = volume > 0 ? 1 / volume : 0f;
                jobs[channel] = new MultiplyJob(_spectrum[channel], multiplier);
                var handle = jobs[channel].Schedule(_spectrum.Count, 1);
                JobHandle.CombineDependencies(_handle, handle);
                handle.Complete();
            }
            _handle.Complete();

            var spectrumListenerData = new List<float[]>();
            for (int channel = 0; channel < channels; channel++)
            {
                spectrumListenerData.Add(jobs[channel].Output.ToArray());
                jobs[channel].Output.Dispose();
            }
            OnSpectrumDataUpdated?.Invoke(
                                          new SpectrumListenerData(audioSource.clip.frequency, numberOfSamples, channels,
                                                                   spectrumListenerData));
        }

        private void CheckSettings(int channels)
        {
            if (_spectrum == null ||
                _spectrum.Count != channels)
                _spectrum = new List<float[]>();

            for (int channel = 0; channel < channels; channel++)
            {
                _spectrum.Add(new float[numberOfSamples]);
            }
        }

        private struct MultiplyJob : IJobParallelFor
        {
            private readonly float _multiplier;

            [ReadOnly] [DeallocateOnJobCompletion]
            private NativeArray<float> _input;

            [WriteOnly]
            private NativeArray<float> _output;

            public MultiplyJob(float[] input, float multiplier)
            {
                _multiplier = multiplier;
                _input = new NativeArray<float>(input, Allocator.TempJob);
                _output = new NativeArray<float>(input.Length, Allocator.TempJob);
            }

            public NativeArray<float> Output => _output;

            public void Execute(int index)
            {
                _output[index] = _input[index] * _multiplier;
            }
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
