using System;
using System.Collections.Generic;
using Base;
using Base.Deque;
using CorePlugin.Cross.Events.Interface;
using CorePlugin.Extensions;
using Modules.AudioAnalyse.Model;
using UnityEngine;

namespace Modules.AudioAnalyse.Systems.BeatDetector
{
    public class BeatAnalyzer : MonoBehaviour, IEventHandler, IEventSubscriber
    {
        [SerializeField] private float analyzeTime = 1f;
        private BPMCalculator _bpmCalculator;
        private Conveyor<List<float>> _fftHistoryBeatDetector;

        private BeatDetectorData _data;

        private List<int> _beatDetectorBandLimits;
        private int _numChannels;

        private int _numberOfSamples;

        private bool _isInitialized;
        private event BeatDetectionEvents.BeatDetectorEvent OnBeatEvent;
        private const int BassLowerLimit = 60;
        private const int BassUpperLimit = 180;
        private const int LowLowerLimit = 500;
        private const int LowUpperLimit = 2000;

        private const int NumBands = 2;

        // private int _windowSize;
        // private float _samplingFrequency;
        
        private class BeatDetectorData
        {
            /// <summary>
            /// Reference to the array containing average values for the sample amplitudes
            /// </summary>
            public float[] AvgSpectrum;

            /// <summary>
            /// Reference to the array containing current samples and amplitudes
            /// </summary>
            public float[] FreqSpectrum;

            /// <summary>
            /// Bool to check if current value is higher than average for bass frequencies
            /// </summary>
            public bool IsBass;

            /// <summary>
            /// Bool to check if current value is higher than average for low-mid frequencies
            /// </summary>
            public bool IsLow;
        }

        public enum BeatType
        {
            Bass = 0,
            Low = 1
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

        private void Deconstruct()
        {
            _isInitialized = false;
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
                for (var index = 0; index < iterator.Count; ++index)
                    avgSpectrum[index] += iterator[index];
            for (var index = 0; index < numBands; ++index) avgSpectrum[index] /= fftHistory.Count;
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
                for (var index = 0; index < iterator.Count; ++index)

                    //Debug.Log("fftresult val is - " + fftResult[index]);
                    varianceSpectrum[index] +=
                        (iterator[index] - avgSpectrum[index]) * (iterator[index] - avgSpectrum[index]);
            for (var index = 0; index < numBands; ++index) varianceSpectrum[index] /= fftHistory.Count;
        }

        /// <summary>
        /// A function to set the booleans for beats by comparing current audio sample with statistical values of previous one's
        /// </summary>
        /// <param name="spectrum"></param>
        /// <param name="referenceData"></param>
        private void GetBeat(IReadOnlyList<float[]> spectrum, ref BeatDetectorData referenceData)
        {
            if (!_isInitialized) return;

            for (var numBand = 0; numBand < NumBands; ++numBand)
            {
                for (var indexFFT = _beatDetectorBandLimits[numBand];
                     indexFFT < _beatDetectorBandLimits[numBand + 1];
                     ++indexFFT)
                {
                    for (var channel = 0; channel < _numChannels; ++channel)
                    {
                        var tempSample = spectrum[channel];
                        referenceData.FreqSpectrum[numBand] += tempSample[indexFFT];
                    }
                }

                referenceData.FreqSpectrum[numBand] /=
                    _beatDetectorBandLimits[numBand + 1] - _beatDetectorBandLimits[numBand] * numBand;
            }

            if (_fftHistoryBeatDetector.Count > 0)
            {
                FillAvgSpectrum(NumBands, ref referenceData.AvgSpectrum, ref _fftHistoryBeatDetector);
                var varianceSpectrum = new float[NumBands];

                FillVarianceSpectrum(NumBands, ref varianceSpectrum, ref referenceData.AvgSpectrum,
                                     ref _fftHistoryBeatDetector);
                const int bass = (int) BeatType.Bass;

                referenceData.IsBass = referenceData.FreqSpectrum[bass] - 0.05 >
                                       BeatThreshold(varianceSpectrum[bass]) * referenceData.AvgSpectrum[bass];
                const int low = (int) BeatType.Low;

                referenceData.IsLow = referenceData.FreqSpectrum[low] - 0.005 >
                                      BeatThreshold(varianceSpectrum[low]) * referenceData.AvgSpectrum[low];
            }
            var fftResult = new List<float>(NumBands);
            for (var index = 0; index < NumBands; ++index) fftResult.Add(referenceData.FreqSpectrum[index]);
            _fftHistoryBeatDetector.AddLast(fftResult);
        }

        private void Initialize(SpectrumProcessorOutput listenerData)
        {
            _numberOfSamples = listenerData.NumberOfSamples;
            var bandSize = listenerData.Frequency / _numberOfSamples; // bandsize = (samplingFrequency / windowSize)
            var fftHistoryMAXSize = listenerData.Frequency / _numberOfSamples;
            _fftHistoryBeatDetector = new Conveyor<List<float>>(fftHistoryMAXSize);

            _beatDetectorBandLimits = new List<int>
                                      {
                                          //bass 60hz-180hz
                                          BassLowerLimit / bandSize,
                                          BassUpperLimit / bandSize,

                                          //low midrange 500hz-2000hz
                                          LowLowerLimit / bandSize,
                                          LowUpperLimit / bandSize
                                      };
            _beatDetectorBandLimits.TrimExcess();
            _numChannels = listenerData.Channels;

            _data = new BeatDetectorData
                    {
                        FreqSpectrum = new float[2],
                        AvgSpectrum = new float[2],
                        IsBass = false,
                        IsLow = false
                    };
            _bpmCalculator = new BPMCalculator(analyzeTime);
            _isInitialized = true;
        }

        private void OnAudioClipChanged()
        {
            Deconstruct();
        }

        private void OnSpectrumReceived(SpectrumProcessorOutput listenerData)
        {
            GetBeat(listenerData.SpectrumData, ref _data);
            OnBeatEvent?.Invoke(GenerateDetectorOutput(_data));
        }

        private BeatDetectorOutput GenerateDetectorOutput(BeatDetectorData data)
        {
            _bpmCalculator.Update(Time.deltaTime, data.IsBass, out var bpmOutput);
            var bpmChanged = bpmOutput.BPMChanged;

            if (data.IsBass)
            {
                Debug.Log("Bass beat");
            }
            
            if (bpmChanged)
            {
                Debug.Log($"BPM: {bpmOutput.BPM} Average BPM: {bpmOutput.AverageBPM}");
            }
            
            return new BeatDetectorOutput(data.AvgSpectrum, data.FreqSpectrum, data.IsBass, data.IsLow, bpmOutput);
        }

        private void SpectrumListenerDataReceived(SpectrumProcessorOutput listenerData)
        {
            if (!_isInitialized)
            {
                Initialize(listenerData);
                OnSpectrumReceived(listenerData);
                return;
            }
            OnSpectrumReceived(listenerData);
        }

        public void InvokeEvents()
        {
        }

        public void Subscribe(params Delegate[] subscribers)
        {
            EventExtensions.Subscribe(ref OnBeatEvent, subscribers);
        }

        public void Unsubscribe(params Delegate[] unsubscribers)
        {
            EventExtensions.Unsubscribe(ref OnBeatEvent, unsubscribers);
        }

        public Delegate[] GetSubscribers()
        {
            return new Delegate[]
                   {
                       (AudioPlayerEvents.AudioClipChangedEvent) OnAudioClipChanged,
                       (DataProcessorsEvents.SpectrumProcessorDataEvent) SpectrumListenerDataReceived
                   };
        }
    }
}
