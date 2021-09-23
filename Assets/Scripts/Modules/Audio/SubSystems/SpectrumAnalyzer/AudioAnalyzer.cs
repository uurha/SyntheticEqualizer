﻿using System;
using System.Collections.Generic;
using Modules.AudioPlayer.Model;
using UnityEngine;

namespace Modules.Audio.SubSystems.SpectrumAnalyzer
{
    public class AudioAnalyzer : MonoBehaviour
    {
        [SerializeField] private BandType bandType;
        [SerializeField] private float fallSpeed = 0.08f;
        [SerializeField] private float sensibility = 8.0f;

        private List<float[]> _levels;
        private List<float[]> _meanLevels;
        private List<float[]> _spectrum;
        private List<float[]> _peakLevels;

        private int _numberOfSamples;
        private Action<List<float[]>> _onAudioAnalyzedDataUpdated;
        private Action _onBeat;

        private BeatDetector _beatDetector;

        private static readonly float[][] middleFrequenciesForBands =
        {
            new[] {125.0f, 500, 1000, 2000},
            new[] {250.0f, 400, 600, 800},
            new[]
            {
                63.0f, 125, 500, 1000, 2000, 4000,
                6000, 8000
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
                800, 1000, 1250, 1600, 2000, 2500,
                3150, 4000, 5000, 6300, 8000, 10000,
                12500, 16000, 20000
            }
        };

        private static readonly float[] bandWidthForBands =
        {
            1.414f, // 2^(1/2)
            1.260f, // 2^(1/3)
            1.414f, // 2^(1/2)
            1.414f, // 2^(1/2)
            1.122f, // 2^(1/6)
            1.122f  // 2^(1/6)
        };

        private int _channels;

        public event Action OnBeatEvent
        {
            add => _onBeat += value;
            remove => _onBeat -= value;
        }

        public event Action<List<float[]>> OnAudioAnalyzedDataUpdated
        {
            add => _onAudioAnalyzedDataUpdated += value;
            remove => _onAudioAnalyzedDataUpdated -= value;
        }

        public enum BandType
        {
            FourBand = 0,
            FourBandVisual = 1,
            EightBand = 2,
            TenBand = 3,
            TwentySixBand = 4,
            ThirtyOneBand = 5
        }

        public bool IsInitialized { get; private set; }

        private void CheckAnalyzedArrays(int channels, int numberOfSamples)
        {
            ValidateArray(ref _spectrum, _channels, _numberOfSamples);
            var bandCount = middleFrequenciesForBands[(int) bandType].Length;
            ValidateArray(ref _levels, _channels, bandCount);
            ValidateArray(ref _peakLevels, _channels, bandCount);
            ValidateArray(ref _meanLevels, _channels, bandCount);
        }

        private void ValidateArray<T>(ref List<T[]> array, int count, int length)
        {
            if (array == null ||
                array.Count != length)
            {
                array = new List<T[]>();
                for (int i = 0; i < count; i++)
                {
                    array.Add(new T[length]);
                }
            }
        }

        private void ComputeSpectrum(SpectrumListenerData data)
        {
            var middleFrequencies = middleFrequenciesForBands[(int) bandType];
            var bandwidth = bandWidthForBands[(int) bandType];
            var fallDown = fallSpeed * Time.deltaTime;
            var filter = Mathf.Exp(-sensibility * Time.deltaTime);

            for (int channel = 0; channel < data.SpectrumData.Count; channel++)
            {
                for (var bi = 0; bi < _levels[channel].Length; bi++)
                {
                    var imin = FrequencyToSpectrumIndex(channel, middleFrequencies[bi] / bandwidth);
                    var imax = FrequencyToSpectrumIndex(channel, middleFrequencies[bi] * bandwidth);
                    var bandMax = 0.0f;
                    for (var fi = imin; fi <= imax; fi++) bandMax = Mathf.Max(bandMax, data.SpectrumData[channel][fi]);
                    _levels[channel][bi] = bandMax;
                    _peakLevels[channel][bi] = Mathf.Max(_peakLevels[channel][bi] - fallDown, bandMax);
                    _meanLevels[channel][bi] = bandMax - (bandMax - _meanLevels[channel][bi]) * filter;
                }
            }
        }

        private int FrequencyToSpectrumIndex(int channel, float f)
        {
            var i = Mathf.FloorToInt(f / AudioSettings.outputSampleRate * 2.0f * _spectrum[channel].Length);
            return Mathf.Clamp(i, 0, _spectrum[channel].Length - 1);
        }

        public void Deconstruct()
        {
            IsInitialized = false;
            _spectrum = new List<float[]>();
            _meanLevels = new List<float[]>();
            _peakLevels = new List<float[]>();
            _levels = new List<float[]>();
        }

        public void OnSpectrumReceived(SpectrumListenerData listenerData)
        {
            if (!IsInitialized) return;
            ComputeSpectrum(listenerData);
            _onAudioAnalyzedDataUpdated?.Invoke(_meanLevels);
            _beatDetector.CalculateBeat(listenerData, out var beatData);
            if (!beatData.isBass) return;
            _onBeat?.Invoke();
        }

        public void InitializeAudio(SpectrumListenerData listenerData)
        {
            _beatDetector = new BeatDetector(listenerData);
            _numberOfSamples = listenerData.NumberOfSamples;
            _channels = listenerData.Channels;
            CheckAnalyzedArrays(_channels, _numberOfSamples);
            IsInitialized = true;
        }
    }
}
