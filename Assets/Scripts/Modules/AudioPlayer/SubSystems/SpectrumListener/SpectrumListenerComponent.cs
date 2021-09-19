using System;
using System.Linq;
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

        [SerializeField] private int channel;
        [SerializeField] private FFTWindow fftWindow = FFTWindow.BlackmanHarris;

        [SettingsHeader]
        [SerializeField] private int numberOfSamples = 1024;

        private JobHandle _handle;
        private float[] _spectrum;

        private event CrossEventsType.OnSpectrumListenerDataUpdateEvent OnSpectrumDataUpdated;

        private void Awake()
        {
            CheckSettings();
        }

        private void Update()
        {
            if (!audioPlayer.IsPlaying) return;
            if (!_handle.IsCompleted) return;
            CheckSettings();
            audioSource.GetSpectrumData(_spectrum, channel, fftWindow);
            var volume = audioSource.volume;
            var multiplier = volume > 0 ? 1 / volume : 0f;
            var job = new MultiplyJob(_spectrum, multiplier);
            _handle = job.Schedule(_spectrum.Length, 1);
            _handle.Complete();

            var spectrumListenerData =
                new SpectrumListenerData(audioSource.clip.frequency, numberOfSamples, channel,
                                         job.Output.ToArray());
            job.Output.Dispose();
            OnSpectrumDataUpdated?.Invoke(spectrumListenerData);
        }

        private void CheckSettings()
        {
            if (_spectrum == null ||
                _spectrum.Length != numberOfSamples)
                _spectrum = new float[numberOfSamples];
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

        public void Subscribe(Delegate[] subscribers)
        {
            OnSpectrumDataUpdated += subscribers.Combine<CrossEventsType.OnSpectrumListenerDataUpdateEvent>();
        }

        public void Unsubscribe(Delegate[] unsubscribers)
        {
            OnSpectrumDataUpdated -= unsubscribers.Combine<CrossEventsType.OnSpectrumListenerDataUpdateEvent>();
        }
    }
}
