using System;
using System.Collections.Generic;
using System.Linq;
using Base;
using CorePlugin.Attributes.Headers;
using CorePlugin.Cross.Events.Interface;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace AudioPlayerModule
{
    public struct SpectrumListenerData
    {
        public int Frequency { get; }
        public int NumberOfSamples { get; }
        public int Channel { get; }
        public float[] SpectrumData { get; }

        public SpectrumListenerData(int frequency, int numberOfSamples, int channel, float[] spectrumData)
        {
            Frequency = frequency;
            NumberOfSamples = numberOfSamples;
            SpectrumData = spectrumData;
            Channel = channel;
        }
    }
    
    [RequireComponent(typeof(AudioSource), typeof(AudioPlayer))]
    public class SpectrumListener : MonoBehaviour, IEventHandler
    {
        [ReferencesHeader]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioPlayer audioPlayer;
        
        [SettingsHeader]
        [SerializeField] private int numberOfSamples = 1024;
        [SerializeField] private int channel = 0;
        [SerializeField] private FFTWindow fftWindow = FFTWindow.BlackmanHarris;
        
        private event CrossEventsType.OnSpectrumListenerDataUpdateEvent OnSpectrumDataUpdated;

        private float[] _spectrum;
        private JobHandle _handle;

        private void Awake()
        {
            CheckSettings();
        }

        public void InvokeEvents()
        {

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
            if (!audioPlayer.IsPlaying) return;

            if (_handle.IsCompleted)
            {
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
        }
        
        private struct MultiplyJob : IJobParallelFor
        {
            private readonly float _multiplier;

            [ReadOnly][DeallocateOnJobCompletion]
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
    }
}
