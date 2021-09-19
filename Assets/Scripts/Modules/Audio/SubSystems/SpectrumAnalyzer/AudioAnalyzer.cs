using System;
using UnityEngine;

namespace Modules.Audio.SubSystems.SpectrumAnalyzer
{
    public class BitRateData
    {
        public long LastT { get; set; }
        public long NowT { get; set; }
        public long Diff { get; set; }
        public long Entries { get; set; }
        public long Sum { get; set; }
    }

    public class AudioAnalyzer : MonoBehaviour
    {
        [SerializeField] private BandType bandType;
        [SerializeField] private float fallSpeed = 0.08f;
        [SerializeField] private float sensibility = 8.0f;
        [SerializeField] private float sensitivity = 0.1f;
        [SerializeField] private int beatCoefficient = 4;

        private bool _analysingState;

        private float[] _levels;
        private float[] _meanLevels;
        private float[] _spectrum;

        private int _numberOfSamples;
        private Action<float[]> _onAudioAnalyzedDataUpdated;
        private Action _onBeat;
        private Action<int> _onBPMChanged;

        private float[] _peakLevels;

        private BeatDetector _beatDetector;

        private int _lastBPM;

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

        public event Action OnBeatEvent
        {
            add => _onBeat += value;
            remove => _onBeat -= value;
        }

        public event Action<int> OnBPMChanged
        {
            add => _onBPMChanged += value;
            remove => _onBPMChanged -= value;
        }

        public event Action<float[]> OnAudioAnalyzedDataUpdated
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

        private void CheckAnalyzedArrays()
        {
            if (_spectrum == null ||
                _spectrum.Length != _numberOfSamples)
                _spectrum = new float[_numberOfSamples];
            var bandCount = middleFrequenciesForBands[(int) bandType].Length;

            if (_levels != null &&
                _levels.Length == bandCount)
                return;
            _levels = new float[bandCount];
            _peakLevels = new float[bandCount];
            _meanLevels = new float[bandCount];
        }

        private void ComputeSpectrum(float[] data)
        {
            var middleFrequencies = middleFrequenciesForBands[(int) bandType];
            var bandwidth = bandWidthForBands[(int) bandType];
            var fallDown = fallSpeed * Time.deltaTime;
            var filter = Mathf.Exp(-sensibility * Time.deltaTime);

            for (var bi = 0; bi < _levels.Length; bi++)
            {
                var imin = FrequencyToSpectrumIndex(middleFrequencies[bi] / bandwidth);
                var imax = FrequencyToSpectrumIndex(middleFrequencies[bi] * bandwidth);
                var bandMax = 0.0f;
                for (var fi = imin; fi <= imax; fi++) bandMax = Mathf.Max(bandMax, data[fi]);
                _levels[bi] = bandMax;
                _peakLevels[bi] = Mathf.Max(_peakLevels[bi] - fallDown, bandMax);
                _meanLevels[bi] = bandMax - (bandMax - _meanLevels[bi]) * filter;
            }
        }

        private int FrequencyToSpectrumIndex(float f)
        {
            var i = Mathf.FloorToInt(f / AudioSettings.outputSampleRate * 2.0f * _spectrum.Length);
            return Mathf.Clamp(i, 0, _spectrum.Length - 1);
        }

        /// <summary>
        /// Use this for initialization
        /// </summary>
        public void InitializeAudio(int samplingRate, int numberOfSamples)
        {
            CheckAnalyzedArrays();
            _lastBPM = 0;

            var data = new BeatDetector.InitializingData(samplingRate, numberOfSamples, beatCoefficient, sensitivity);
            
            _beatDetector = new BeatDetector(data);
            _numberOfSamples = numberOfSamples;
            IsInitialized = true;
        }

        public void OnSpectrumReceived(float[] spectrum)
        {
            if (!IsInitialized) return;
            if (!_analysingState) return;
            _spectrum = spectrum;
            ComputeSpectrum(_spectrum);
            _onAudioAnalyzedDataUpdated?.Invoke(_meanLevels);
            var beatDetected = _beatDetector.CalculateBeat(_spectrum, out var bpm);
            if (beatDetected) _onBeat?.Invoke();
            if (_lastBPM == bpm) return;
            _lastBPM = bpm;
            _onBPMChanged?.Invoke(_lastBPM);
        }

        public void SetStateAnalyzing(bool state)
        {
            _analysingState = state;
        }
    }
}
