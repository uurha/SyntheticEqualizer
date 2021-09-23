using System;
using System.Collections.Generic;
using Base.Deque;
using Modules.AudioPlayer.Model;
using UnityEngine;

namespace Modules.Audio.SubSystems.SpectrumAnalyzer
{
    public class BeatDetector
    {
        private const int BassLowerLimit = 60;
        private const int BassUpperLimit = 180;
        private const int LowLowerLimit = 500;
        private const int LowUpperLimit = 2000;

        private const int NumBands = 2;

        private int _windowSize;
        private float _samplingFrequency;

        private Conveyor<List<float>> _fftHistoryBeatDetector;

        private BeatAnalyzeData _data;

        private readonly List<int> _beatDetectorBandLimits;
        private readonly int _numChannels;

        private readonly int _numberOfSamples = 1024;

        public class BeatAnalyzeData
        {
            /// <summary>
            /// Reference to the array containing current samples and amplitudes
            /// </summary>
            public float[] freqSpectrum;
            /// <summary>
            /// Reference to the array containing average values for the sample amplitudes
            /// </summary>
            public float[] avgSpectrum;
            /// <summary>
            /// Bool to check if current value is higher than average for bass frequencies
            /// </summary>
            public bool isBass;
            /// <summary>
            /// Bool to check if current value is higher than average for low-mid frequencies
            /// </summary>
            public bool isLow;
        }

        public BeatDetector(SpectrumListenerData listenerData)
        {
            var bandsize = listenerData.Frequency / _numberOfSamples; // bandsize = (samplingFrequency / windowSize)
            var fftHistoryMAXSize = listenerData.Frequency / _numberOfSamples;
            _numberOfSamples = listenerData.NumberOfSamples;
            _fftHistoryBeatDetector = new Conveyor<List<float>>(fftHistoryMAXSize);
            _beatDetectorBandLimits = new List<int>();
            _beatDetectorBandLimits.Clear();

            //bass 60hz-180hz
            _beatDetectorBandLimits.Add(BassLowerLimit / bandsize);
            _beatDetectorBandLimits.Add(BassUpperLimit / bandsize);

            //low midrange 500hz-2000hz
            _beatDetectorBandLimits.Add(LowLowerLimit / bandsize);
            _beatDetectorBandLimits.Add(LowUpperLimit / bandsize);
            _beatDetectorBandLimits.TrimExcess();
            _fftHistoryBeatDetector.Clear();
            _numChannels = listenerData.Channels;

            _data = new BeatAnalyzeData()
                    {
                        freqSpectrum = new float[4],
                        avgSpectrum = new float[4],
                        isBass = false,
                        isLow = false
                    };
        }

        public void CalculateBeat(SpectrumListenerData listenerData, out BeatAnalyzeData data)
        {
            GetBeat(listenerData.SpectrumData, ref _data);
            data = _data;
        }

        /// <summary>
        /// A function to set the booleans for beats by comparing current audio sample with statistical values of previous one's
        /// </summary>
        /// <param name="spectrum"></param>
        /// <param name="referenceData"></param>
        private void GetBeat(IReadOnlyList<float[]> spectrum, ref BeatAnalyzeData referenceData)
        {
            for (var numBand = 0; numBand < NumBands; ++numBand)
            {
                for (var indexFFT = _beatDetectorBandLimits[numBand];
                     indexFFT < _beatDetectorBandLimits[numBand + 1];
                     ++indexFFT)
                {
                    for (var channel = 0; channel < _numChannels; ++channel)
                    {
                        var tempSample = spectrum[channel];
                        referenceData.freqSpectrum[numBand] += tempSample[indexFFT];
                    }
                }

                referenceData.freqSpectrum[numBand] /=
                    (_beatDetectorBandLimits[numBand + 1] - _beatDetectorBandLimits[numBand] * numBand);
            }

            if (_fftHistoryBeatDetector.Count > 0)
            {
                FillAvgSpectrum(NumBands, ref referenceData.avgSpectrum, ref _fftHistoryBeatDetector);
                var varianceSpectrum = new float[NumBands];
                FillVarianceSpectrum(NumBands, ref varianceSpectrum, ref referenceData.avgSpectrum, ref _fftHistoryBeatDetector);
                referenceData.isBass = (referenceData.freqSpectrum[0] - 0.05) > BeatThreshold(varianceSpectrum[0]) * referenceData.avgSpectrum[0];
                referenceData.isLow = (referenceData.freqSpectrum[1] - 0.005) > BeatThreshold(varianceSpectrum[1]) * referenceData.avgSpectrum[1];
            }
            var fftResult = new List<float>(NumBands);

            for (var index = 0; index < NumBands; ++index)
            {
                fftResult.Add(referenceData.freqSpectrum[index]);
            }
            _fftHistoryBeatDetector.AddLast(fftResult);
        }

        /// <summary>
        /// Function to add average values to the array
        /// </summary>
        /// <param name="numBands"></param>
        /// <param name="avgSpectrum"></param>
        /// <param name="fftHistory"></param>
        private void FillAvgSpectrum(int numBands, ref float[] avgSpectrum, ref Conveyor<List<float>> fftHistory)
        {
            foreach (var iterator in fftHistory)
            {
                for (var index = 0; index < iterator.Count; ++index)
                {
                    avgSpectrum[index] += iterator[index];
                }
            }

            for (var index = 0; index < numBands; ++index)
            {
                avgSpectrum[index] /= (fftHistory.Count);
            }
        }

        /// <summary>
        /// Function to add variance values to the array
        /// </summary>
        /// <param name="numBands"></param>
        /// <param name="varianceSpectrum"></param>
        /// <param name="avgSpectrum"></param>
        /// <param name="fftHistory"></param>
        private void FillVarianceSpectrum(int numBands, ref float[] varianceSpectrum,
                                          ref float[] avgSpectrum, ref Conveyor<List<float>> fftHistory)
        {
            foreach (var iterator in fftHistory)
            {
                for (var index = 0; index < iterator.Count; ++index)
                {
                    //Debug.Log("fftresult val is - " + fftResult[index]);
                    varianceSpectrum[index] +=
                        (iterator[index] - avgSpectrum[index]) * (iterator[index] - avgSpectrum[index]);
                }
            }

            for (var index = 0; index < numBands; ++index)
            {
                varianceSpectrum[index] /= (fftHistory.Count);
            }
        }

        /// <summary>
        /// Function to get the threshold value for the sample
        /// </summary>
        /// <param name="variance">variance for the sample</param>
        /// <returns>float threshold</returns>
        private static float BeatThreshold(float variance)
        {
            return -15f * variance + 1.55f;
        }
    }
}
