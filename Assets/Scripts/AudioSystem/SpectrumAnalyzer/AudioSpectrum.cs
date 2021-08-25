// Audio spectrum component
// By Keijiro Takahashi, 2013
// https://github.com/keijiro/unity-audio-spectrum

using System;
using System.Collections.Generic;
using System.Linq;
using Base;
using CorePlugin.Cross.Events.Interface;
using UnityEngine;

namespace AudioSystem.SpectrumAnalyzer
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioSpectrum : MonoBehaviour, IEventHandler
    {
        public enum BandType
        {
            FourBand,
            FourBandVisual,
            EightBand,
            TenBand,
            TwentySixBand,
            ThirtyOneBand
        };

        private static readonly float[][] MiddleFrequenciesForBands =
        {
            new[] {125.0f, 500, 1000, 2000},
            new[] {250.0f, 400, 600, 800},
            new[]
            {
                63.0f, 125, 500, 1000, 2000, 4000, 6000,
                8000
            },
            new[]
            {
                31.5f, 63, 125, 250, 500, 1000, 2000,
                4000, 8000, 16000
            },
            new[]
            {
                25.0f, 31.5f, 40, 50, 63, 80, 100, 125,
                160, 200, 250, 315, 400, 500, 630, 800,
                1000, 1250, 1600, 2000, 2500, 3150,
                4000, 5000, 6300, 8000
            },
            new[]
            {
                20.0f, 25, 31.5f, 40, 50, 63, 80, 100,
                125, 160, 200, 250, 315, 400, 500, 630,
                800, 1000, 1250, 1600, 2000, 2500, 3150,
                4000, 5000, 6300, 8000, 10000, 12500,
                16000, 20000
            },
        };

        private static readonly float[] BandwidthForBands =
        {
            1.414f, // 2^(1/2)
            1.260f, // 2^(1/3)
            1.414f, // 2^(1/2)
            1.414f, // 2^(1/2)
            1.122f, // 2^(1/6)
            1.122f  // 2^(1/6)
        };

        [SerializeField] private AudioSource audioSource;
        [SerializeField] private int numberOfSamples = 1024;
        [SerializeField] private BandType bandType = BandType.TenBand;
        [SerializeField] private float fallSpeed = 0.08f;
        [SerializeField] private float sensibility = 8.0f;

        private event CrossEventsType.AudioLevelsUpdated OnLevelsUpdated;
        private event CrossEventsType.AudioPeakLevelsUpdated OnPeakLevelsUpdated;
        private event CrossEventsType.AudioMeanLevelsUpdated OnMeanLevelsUpdated;

        private float[] _rawSpectrum;
        private float[] _levels;
        private float[] _peakLevels;
        private float[] _meanLevels;

        public float[] Levels => _levels;

        public float[] PeakLevels => _peakLevels;

        public float[] MeanLevels => _meanLevels;

        public int NumberOfSamples
        {
            get => numberOfSamples;
            set => numberOfSamples = value;
        }

        public BandType Type
        {
            get => bandType;
            set => bandType = value;
        }

        public float FallSpeed
        {
            get => fallSpeed;
            set => fallSpeed = value;
        }

        public float Sensibility
        {
            get => sensibility;
            set => sensibility = value;
        }

        public AudioSource Source
        {
            get => audioSource;
            set => audioSource = value;
        }

        private void CheckBuffers()
        {
            if (_rawSpectrum == null ||
                _rawSpectrum.Length != numberOfSamples)
            {
                _rawSpectrum = new float[numberOfSamples];
            }
            var bandCount = MiddleFrequenciesForBands[(int) bandType].Length;

            if (_levels != null &&
                _levels.Length == bandCount)
                return;
            _levels = new float[bandCount];
            _peakLevels = new float[bandCount];
            _meanLevels = new float[bandCount];
        }

        private int FrequencyToSpectrumIndex(float f)
        {
            var i = Mathf.FloorToInt(f / AudioSettings.outputSampleRate * 2.0f * _rawSpectrum.Length);
            return Mathf.Clamp(i, 0, _rawSpectrum.Length - 1);
        }

        private void Awake()
        {
            CheckBuffers();
        }

        private void Update()
        {
            CheckBuffers();

            audioSource.GetSpectrumData(_rawSpectrum, 0, FFTWindow.BlackmanHarris);

            var middleFrequencies = MiddleFrequenciesForBands[(int) bandType];
            var bandwidth = BandwidthForBands[(int) bandType];

            var fallDown = fallSpeed * Time.deltaTime;
            var filter = Mathf.Exp(-sensibility * Time.deltaTime);

            for (var bi = 0; bi < _levels.Length; bi++)
            {
                var imin = FrequencyToSpectrumIndex(middleFrequencies[bi] / bandwidth);
                var imax = FrequencyToSpectrumIndex(middleFrequencies[bi] * bandwidth);

                var bandMax = 0.0f;

                for (var fi = imin; fi <= imax; fi++)
                {
                    bandMax = Mathf.Max(bandMax, _rawSpectrum[fi]);
                }

                _levels[bi] = bandMax;
                _peakLevels[bi] = Mathf.Max(_peakLevels[bi] - fallDown, bandMax);
                _meanLevels[bi] = bandMax - (bandMax - _meanLevels[bi]) * filter;
            }
            OnLevelsUpdated?.Invoke(_levels);
            OnPeakLevelsUpdated?.Invoke(_peakLevels);
            OnMeanLevelsUpdated?.Invoke(_meanLevels);
        }

        public void InvokeEvents()
        {
            
        }

        public void Subscribe(IEnumerable<Delegate> subscribers)
        {
            foreach (var levelsUpdated in subscribers.OfType<CrossEventsType.AudioLevelsUpdated>())
            {
                OnLevelsUpdated += levelsUpdated;
            }
            foreach (var meanLevelsUpdated in subscribers.OfType<CrossEventsType.AudioMeanLevelsUpdated>())
            {
                OnMeanLevelsUpdated += meanLevelsUpdated;
            }
            foreach (var peakLevelsUpdated in subscribers.OfType<CrossEventsType.AudioPeakLevelsUpdated>())
            {
                OnPeakLevelsUpdated += peakLevelsUpdated;
            }
        }

        public void Unsubscribe(IEnumerable<Delegate> unsubscribers)
        {
            foreach (var levelsUpdated in unsubscribers.OfType<CrossEventsType.AudioLevelsUpdated>())
            {
                OnLevelsUpdated -= levelsUpdated;
            }
            foreach (var meanLevelsUpdated in unsubscribers.OfType<CrossEventsType.AudioMeanLevelsUpdated>())
            {
                OnMeanLevelsUpdated -= meanLevelsUpdated;
            }
            foreach (var peakLevelsUpdated in unsubscribers.OfType<CrossEventsType.AudioPeakLevelsUpdated>())
            {
                OnPeakLevelsUpdated -= peakLevelsUpdated;
            }
        }
    }
}