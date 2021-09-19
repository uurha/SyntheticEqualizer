using System;
using UnityEngine;

namespace Modules.Audio.SubSystems.SpectrumAnalyzer
{
    public class BeatDetector
    {
        public struct InitializingData
        {
            public int SamplingRate { get; }
            public int NumberOfSamples { get; }
            public int BeatCoefficient { get; }
            public float Sensitivity { get; }
            
            public float BandWidth { get; }

            public InitializingData(int samplingRate, int numberOfSamples, int beatCoefficient, float sensitivity)
            {
                SamplingRate = samplingRate;
                NumberOfSamples = numberOfSamples;
                BeatCoefficient = beatCoefficient;
                Sensitivity = sensitivity;
                BandWidth = 2f / numberOfSamples * (samplingRate / 2f);
            }
        }
        
        // log-frequency averaging controls 
        private readonly int _blipDelayLen = 16;

        private readonly int _colMax = 120;

        // smoothing constant for running average
        private readonly float _decay = 0.997f;

        // (in frames) largest lag to track
        private readonly int _maxlag = 100;
        private float[] _actualValues;
        private float _alpha;
        private AutoCorrelation _autoCorrelation;
        private float[] _averages;
        private int _beatsBand = 12;

        private BitRateData _bitRateData;
        private InitializingData _data;
        
        // counter to suppress double-beats
        private int[] _blipDelay;
        private float[] _doBeat;
        private readonly float _framePeriod;
        private int _now;
        private float[] _onsets;
        private float[] _scoreFun;
        private int _sinceLast;
        private float[] _spec;

        public BeatDetector(int samplingRate, int numberOfSamples, int beatCoefficient, float sensitivity)
        {
            InitArrays();
            _data = new InitializingData(samplingRate, numberOfSamples, beatCoefficient, sensitivity);
            
            _bitRateData = new BitRateData {LastT = GetCurrentTimeMillis()};
            _framePeriod = _data.NumberOfSamples / (float) _data.SamplingRate;
            _autoCorrelation = new AutoCorrelation(_maxlag, _decay, _framePeriod, _data.BandWidth);
            for (var i = 0; i < _beatsBand; ++i) _spec[i] = 100.0f;
        }
        
        public BeatDetector(InitializingData data)
        {
            InitArrays();
            _data = data;
            
            _bitRateData = new BitRateData {LastT = GetCurrentTimeMillis()};
            _framePeriod = _data.NumberOfSamples / (float) _data.SamplingRate;
            _autoCorrelation = new AutoCorrelation(_maxlag, _decay, _framePeriod, _data.BandWidth);
            for (var i = 0; i < _beatsBand; ++i) _spec[i] = 100.0f;
        }

        public float Constrain(float value, float inclusiveMinimum, float inclusiveMaximum)
        {
            if (!(value >= inclusiveMinimum)) return inclusiveMinimum;
            return value <= inclusiveMaximum ? value : inclusiveMaximum;
        }

        private float Map(float s, float a1, float a2, float b1, float b2)
        {
            return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
        }

        private long GetCurrentTimeMillis()
        {
            var milliseconds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            return milliseconds;
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

        private void InitArrays()
        {
            _averages = new float[_beatsBand];
            _spec = new float[_beatsBand];
            _blipDelay = new int[_blipDelayLen];
            _onsets = new float[_colMax];
            _scoreFun = new float[_colMax];
            _doBeat = new float[_colMax];
            _actualValues = new float[_maxlag];
            _averages = new float[_beatsBand];
            _alpha = 100 * _data.Sensitivity;
        }

        public bool CalculateBeat(float[] spectrum, out int bpm)
        {
            ComputeAverages(spectrum);

            float onset = 0;

            for (var i = 0; i < _beatsBand; i++)
            {
                var specVal =
                    Math.Max(-100.0f, 20.0f * (float) Math.Log10(_averages[i]) + 160);
                specVal *= 0.025f;
                var dbInc = specVal - _spec[i];
                _spec[i] = specVal;
                onset += dbInc;
            }
            _onsets[_now] = onset;
            
            _autoCorrelation.NewVal(onset);

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

                _actualValues[_maxlag - 1 - i] = acVal;
            }
            
            var smax = float.MinValue;
            var smaxix = 0;

            for (var i = tempopd / 2; i < Math.Min(_colMax, 2 * tempopd); ++i)
            {
                var score = onset + _scoreFun[(_now - i + _colMax) % _colMax] -
                            _alpha * (float) Math.Pow(Math.Log(i / (float) tempopd), 2);

                if (score > smax)
                {
                    smax = score;
                    smaxix = i;
                }
            }
            
            _scoreFun[_now] = smax;
            var smin = _scoreFun[0];
            smaxix = 0;
            
            for (var i = 0; i < _colMax; ++i)
                if (_scoreFun[i] < smin)
                    smin = _scoreFun[i];
            for (var i = 0; i < _colMax; ++i) _scoreFun[i] -= smin;
            smax = _scoreFun[0];

            for (var i = 0; i < _colMax; ++i)
                if (_scoreFun[i] > smax)
                {
                    smax = _scoreFun[i];
                    smaxix = i;
                }
            _doBeat[_now] = 0;
            ++_sinceLast;

            var beat = false;
            if (smaxix == _now)
                if (_sinceLast > tempopd / _data.BeatCoefficient)
                {
                    beat = true;
                    _blipDelay[0] = 1;

                    _doBeat[_now] = 1;
                    
                    _sinceLast = 0;
                }
            
            if (++_now == _colMax) _now = 0;
            bpm = Mathf.RoundToInt(60 / (tempopd * _framePeriod));

            return beat;
        }

        private void ComputeAverages(float[] data)
        {
            for (var i = 0; i < _beatsBand; i++)
            {
                float avg = 0;
                int lowFreq;

                if (i == 0)
                    lowFreq = 0;
                else
                    lowFreq = (int) (Mathf.RoundToInt(_data.SamplingRate / 2f) / (float) Math.Pow(2, 12 - i));
                var hiFreq = (int) (Mathf.RoundToInt(_data.SamplingRate / 2f) / (float) Math.Pow(2, 11 - i));
                var lowBound = FreqToIndex(lowFreq);
                var hiBound = FreqToIndex(hiFreq);
                for (var j = lowBound; j <= hiBound; j++) avg += data[j];
                
                avg /= hiBound - lowBound + 1;
                _averages[i] = avg;
            }
        }

        private int FreqToIndex(int freq)
        {
            if (freq < _data.BandWidth / 2) return 0;

            if (freq > Mathf.RoundToInt(_data.SamplingRate / 2f) - _data.BandWidth / 2) return _data.NumberOfSamples / 2;

            var fraction = freq / (float) _data.SamplingRate;
            var i = (int) Math.Round(_data.NumberOfSamples * fraction);
            return i;
        }
    }
}
