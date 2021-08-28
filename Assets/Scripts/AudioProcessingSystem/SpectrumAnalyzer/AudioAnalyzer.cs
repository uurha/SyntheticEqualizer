using System;
using AudioPlayerSystem.Interfaces;
using UnityEngine;

namespace AudioSystem.SpectrumAnalyzer
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
        public enum BandType
        {
            FourBand = 0,
            FourBandVisual = 1,
            EightBand = 2,
            TenBand = 3,
            TwentySixBand = 4,
            ThirtyOneBand = 5
        };

        private static float[][] middleFrequenciesForBands =
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
            },
        };

        private static float[] bandWidthForBands =
        {
            1.414f, // 2^(1/2)
            1.260f, // 2^(1/3)
            1.414f, // 2^(1/2)
            1.414f, // 2^(1/2)
            1.122f, // 2^(1/6)
            1.122f  // 2^(1/6)
        };

        [SerializeField] private BandType bandType;
        [SerializeField] private int numberOfSamples = 1024;
        [SerializeField] private float sensitivity = 0.1f;
        [SerializeField] private float sensibility = 8.0f;
        [SerializeField] private float fallSpeed = 0.08f;

        private IAudioPlayer _audioPlayer;
        private bool isInitialized;
        
        private float[] _levels;
        private float[] _peakLevels;
        private float[] _meanLevels;

        // fft sampling frequency
        private int _samplingRate = 44100;

        // log-frequency averaging controls 

        private int _blipDelayLen = 16;
        private int[] _blipDelay;

        // counter to suppress double-beats
        private int _sinceLast = 0;

        private float _framePeriod;

        // storage space 
        private int _colMax = 120;
        private float[] _spectrum;
        private float[] _actualValues;
        private float[] _onsets;
        private float[] _scoreFun;
        private float[] _doBeat;

        // time index for circular buffer within above
        private int _now = 0;

        // the spectrum of the previous step
        private float[] _spec;

        /* Autocorrelation structure */
        // (in frames) largest lag to track
        private int _maxlag = 100;

        // smoothing constant for running average
        private float _decay = 0.997f;
        private AutoCorrelation _autoCorrelation;

        // trade-off constant between tempo deviation penalty and onset strength
        private float _alpha;

        private int _lastBPM;

        private Action _onBeat;
        private Action<float[]> _onSpectrum;
        private Action<int> _onBPMChanged;

        private BitRateData _bitRateData;

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

        public event Action<float[]> OnSpectrumEvent
        {
            add => _onSpectrum += value;
            remove => _onSpectrum -= value;
        }

        private long GetCurrentTimeMillis()
        {
            var milliseconds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            return milliseconds;
        }

        private void InitArrays()
        {
            _blipDelay = new int[_blipDelayLen];
            _onsets = new float[_colMax];
            _scoreFun = new float[_colMax];
            _doBeat = new float[_colMax];

            _actualValues = new float[_maxlag];

            CheckAnalyzedArrays();
            _alpha = 100 * sensitivity;
        }

        private void CheckAnalyzedArrays()
        {
            if (_spectrum == null ||
                _spectrum.Length != numberOfSamples)
            {
                _spectrum = new float[numberOfSamples];
            }
            var bandCount = middleFrequenciesForBands[(int) bandType].Length;

            if (_levels != null &&
                _levels.Length == bandCount)
                return;

            _levels = new float[bandCount];
            _peakLevels = new float[bandCount];
            _meanLevels = new float[bandCount];
        }

        public void ReinitializeAudio()
        {
            if (!isInitialized) return;
            isInitialized = false;
            InitializeAudio(_audioPlayer);
        }

        /// <summary>
        /// Use this for initialization
        /// </summary>
        public void InitializeAudio(IAudioPlayer audioSource)
        {
            _audioPlayer = audioSource;
            InitArrays();
            _bitRateData = new BitRateData {LastT = GetCurrentTimeMillis()};
            _samplingRate = audioSource.Clip.frequency;
            _framePeriod = numberOfSamples / (float) _samplingRate;

            // initialize record of previous spectrum
            var bandCount = middleFrequenciesForBands[(int) bandType].Length;
            _spec = new float[bandCount];
            for (var i = 0; i < bandCount; ++i) _spec[i] = 100.0f;
            var bandWidth = bandWidthForBands[(int) bandType];
            _autoCorrelation = new AutoCorrelation(_maxlag, _decay, _framePeriod, bandWidth);
            isInitialized = true;
        }

        public void TapTempo()
        {
            _bitRateData.NowT = GetCurrentTimeMillis();
            _bitRateData.Diff = _bitRateData.NowT - _bitRateData.LastT;
            _bitRateData.LastT = _bitRateData.NowT;
            _bitRateData.Sum += _bitRateData.Diff;
            _bitRateData.Entries++;
            var average = (int) (_bitRateData.Sum / _bitRateData.Entries);
            Debug.Log("average = " + average);
        }

        private void Update()
        {
            if (!isInitialized) return;
            if (!_audioPlayer.IsPlaying) return;
            _audioPlayer.GetSpectrumData(_spectrum, 0, FFTWindow.BlackmanHarris);
            ComputeAverages(_spectrum);
            _onSpectrum?.Invoke(_meanLevels);

            // calculate the value of the onset function in this frame 
            float onset = 0;

            var bandCount = middleFrequenciesForBands[(int) bandType].Length;

            for (var i = 0; i < bandCount; i++)
            {
                var specVal =
                    Math.Max(-100.0f,
                             20.0f * (float) Math.Log10(_meanLevels[i]) + 160); // dB value of this band
                specVal *= 0.025f;

                // dB increment since last frame
                var dbInc = specVal - _spec[i];

                // record this from to use next time around
                _spec[i] = specVal;

                // onset function is the sum of dB increments        
                onset += dbInc;
            }
            _onsets[_now] = onset;

            // update auto correlator and find peak lag = current tempo 
            _autoCorrelation.NewVal(onset);

            // record largest value in (weighted) autocorrelation as it will be the tempo
            var aMax = 0.0f;
            var tempopd = 0;

            for (var i = 0; i < _maxlag; ++i)
            {
                var acVal = (float) Math.Sqrt(_autoCorrelation.AutoCorrelator(i));

                if (acVal > aMax)
                {
                    aMax = acVal;
                    tempopd = i;
                }

                // store in array backwards, so it displays right-to-left, in line with traces
                _actualValues[_maxlag - 1 - i] = acVal;
            }

            // calculate DP-ish function to update the best-score function 
            float sMax = -999999;
            int smaxix;

            // weight can be varied dynamically with the mouse
            _alpha = 100 * sensitivity;

            // consider all possible preceding beat times from 0.5 to 2.0 x current tempo period
            for (var i = tempopd / 2; i < Math.Min(_colMax, 2 * tempopd); ++i)
            {
                // objective function - this beat's cost + score to last beat + transition penalty
                var score = onset + _scoreFun[(_now - i + _colMax) % _colMax] -
                            _alpha * (float) Math.Pow(Math.Log(i / (float) tempopd), 2);

                // keep track of the best-scoring predecessor
                if (!(score > sMax)) continue;
                sMax = score;
            }
            _scoreFun[_now] = sMax;

            // keep the smallest value in the score fn window as zero, by substring the min val
            var sMin = _scoreFun[0];

            for (var i = 0; i < _colMax; ++i)
                if (_scoreFun[i] < sMin)
                    sMin = _scoreFun[i];
            for (var i = 0; i < _colMax; ++i) _scoreFun[i] -= sMin;

            // find the largest value in the score fn window, to decide if we emit a blip 
            sMax = _scoreFun[0];
            smaxix = 0;

            for (var i = 0; i < _colMax; ++i)
            {
                if (!(_scoreFun[i] > sMax)) continue;
                sMax = _scoreFun[i];
                smaxix = i;
            }

            // do beat array records where we actually place beats
            _doBeat[_now] = 0; // default is no beat this frame
            ++_sinceLast;

            // if current value is largest in the array, probably means we're on a beat
            if (smaxix == _now)
            {
                // TapTempo();
                // make sure the most recent beat wasn't too recently
                if (_sinceLast > tempopd / 4)
                {
                    _onBeat?.Invoke();
                    _blipDelay[0] = 1;

                    // record that we did actually mark a beat this frame
                    _doBeat[_now] = 1;

                    // reset counter of frames since last beat
                    _sinceLast = 0;
                }
            }

            // update column index (for ring buffer) 
            if (++_now == _colMax) _now = 0;

            var bpm = (int) Math.Round(60 / (tempopd * _framePeriod));

            if (_lastBPM == bpm) return;
            _lastBPM = bpm;
            _onBPMChanged?.Invoke(_lastBPM);
        }

        private int FrequencyToSpectrumIndex(float f)
        {
            var i = Mathf.FloorToInt(f / AudioSettings.outputSampleRate * 2.0f * _spectrum.Length);
            return Mathf.Clamp(i, 0, _spectrum.Length - 1);
        }

        private void ComputeAverages(float[] data)
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

                for (var fi = imin; fi <= imax; fi++)
                {
                    bandMax = Mathf.Max(bandMax, data[fi]);
                }

                _levels[bi] = bandMax;
                _peakLevels[bi] = Mathf.Max(_peakLevels[bi] - fallDown, bandMax);
                _meanLevels[bi] = bandMax - (bandMax - _meanLevels[bi]) * filter;
            }
        }

        private float Map(float s, float a1, float a2, float b1, float b2)
        {
            return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
        }

        public float Constrain(float value, float inclusiveMinimum, float inclusiveMaximum)
        {
            if (!(value >= inclusiveMinimum)) return inclusiveMinimum;
            return value <= inclusiveMaximum ? value : inclusiveMaximum;
        }

        // class to compute an array of online auto correlators
        private class AutoCorrelation
        {
            private readonly int _delLength;
            private readonly float _decay;
            private readonly float[] _delays;
            private readonly float[] _outputs;
            private int _index;

            private readonly float[] _weight;
            private readonly float _weightMiddleBPM = 120f;

            public AutoCorrelation(int len, float decay, float framePeriod, float bandwidth)
            {
                _decay = decay;
                _delLength = len;
                _delays = new float[_delLength];
                _outputs = new float[_delLength];
                _index = 0;

                // calculate a log-lag gaussian weighting function, to prefer tempi around 120 bpm
                _weight = new float[_delLength];

                for (var i = 0; i < _delLength; ++i)
                {
                    var bpm = 60.0f / (framePeriod * i);

                    // weighting is Gaussian on log-BPM axis, centered at weight middle BPM, SD = woctavewidth octaves
                    _weight[i] =
                        (float) Math.Exp(-0.5f * Math.Pow(Math.Log(bpm / _weightMiddleBPM) / Math.Log(2.0f) / bandwidth,
                                                          2.0f));
                }
            }

            public void NewVal(float val)
            {
                _delays[_index] = val;

                // update running auto correlator values
                for (var i = 0; i < _delLength; ++i)
                {
                    var delix = (_index - i + _delLength) % _delLength;
                    _outputs[i] += (1 - _decay) * (_delays[_index] * _delays[delix] - _outputs[i]);
                }
                if (++_index == _delLength) _index = 0;
            }

            // read back the current auto correlator value at a particular lag
            public float AutoCorrelator(int del)
            {
                var correlated = _weight[del] * _outputs[del];
                return correlated;
            }
        }
    }
}
