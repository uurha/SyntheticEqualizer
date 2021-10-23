using System;
using System.Collections.Generic;
using Base;
using CorePlugin.Cross.Events.Interface;
using CorePlugin.Extensions;
using Extensions;
using Modules.AudioAnalyse.Model;
using Modules.AudioPlayerUI.Model;
using UnityEngine;

namespace Modules.AudioAnalyse.Systems.SpectrumAnalyse
{
    public class SpectrumAnalyzer : MonoBehaviour, IEventHandler, IEventSubscriber
    {
        private List<float[]> _levels;
        private List<float[]> _meanLevels;
        private List<float[]> _spectrum;
        private List<float[]> _peakLevels;

        private int _numberOfSamples;

        private int _channels;
        private bool _isInitialized;

        private SpectrumAnalyzerSettings _currentSettings;

        private event AudioAnalyzerEvents.SpectrumAnalyzerDataEvent OnAudioAnalyzedDataEvent;

        private void CheckAnalyzedArrays(int channels, int numberOfSamples)
        {
            ValidateListArray(ref _spectrum, channels, numberOfSamples);
            var bandCount = _currentSettings.BandType.GetBandLenght();
            ValidateListArray(ref _levels, channels, bandCount);
            ValidateListArray(ref _peakLevels, channels, bandCount);
            ValidateListArray(ref _meanLevels, channels, bandCount);
        }

        private void ComputeSpectrum(SpectrumProcessorOutput data)
        {
            var fallDown = _currentSettings.FallSpeed * Time.deltaTime;
            var filter = Mathf.Exp(-_currentSettings.Sensibility * Time.deltaTime);

            for (var channel = 0; channel < data.SpectrumData.Count; channel++)
            {
                var length = _levels[channel].Length;

                for (var bi = 0; bi < length; bi++)
                {
                    var imin = AudioExtensions.FrequencyToSpectrumMinIndex(length, bi, _currentSettings.BandType);
                    var imax = AudioExtensions.FrequencyToSpectrumMaxIndex(length, bi, _currentSettings.BandType);
                    var bandMax = 0.0f;
                    for (var fi = imin; fi <= imax; fi++) bandMax = Mathf.Max(bandMax, data.SpectrumData[channel][fi]);
                    _levels[channel][bi] = bandMax;
                    _peakLevels[channel][bi] = Mathf.Max(_peakLevels[channel][bi] - fallDown, bandMax);
                    _meanLevels[channel][bi] = bandMax - (bandMax - _meanLevels[channel][bi]) * filter;
                }
            }
        }

        private void Deconstruct()
        {
            _isInitialized = false;
            _spectrum = new List<float[]>();
            _meanLevels = new List<float[]>();
            _peakLevels = new List<float[]>();
            _levels = new List<float[]>();
        }

        private SpectrumAnalyzerOutput GenerateAnalyzerData(SpectrumProcessorOutput listenerData)
        {
            return new SpectrumAnalyzerOutput(listenerData, _meanLevels, _peakLevels, _levels);
        }

        private void Initialize(SpectrumProcessorOutput listenerData)
        {
            _numberOfSamples = listenerData.NumberOfSamples;
            _channels = listenerData.Channels;
            CheckAnalyzedArrays(_channels, _numberOfSamples);
            _isInitialized = true;
        }

        private void OnAnalyzerSettingsChanged(SpectrumAnalyzerSettings settings)
        {
            _currentSettings = settings;
        }

        private void OnAudioClipChanged()
        {
            Deconstruct();
        }

        private void ProcessSpectrumData(SpectrumProcessorOutput listenerData)
        {
            if (!_isInitialized) return;
            ComputeSpectrum(listenerData);
            OnAudioAnalyzedDataEvent?.Invoke(GenerateAnalyzerData(listenerData));
        }

        private void SpectrumListenerDataReceived(SpectrumProcessorOutput listenerData)
        {
            if (!_currentSettings.IsValid) return;

            if (!_isInitialized)
            {
                Initialize(listenerData);
                ProcessSpectrumData(listenerData);
                return;
            }
            ProcessSpectrumData(listenerData);
        }

        private void ValidateListArray<T>(ref List<T[]> array, int count, int length)
        {
            if (array != null &&
                array.Count == length)
                return;
            array = new List<T[]>();
            for (var i = 0; i < count; i++) array.Add(new T[length]);
        }

        public void InvokeEvents()
        {
        }

        public void Subscribe(params Delegate[] subscribers)
        {
            EventExtensions.Subscribe(ref OnAudioAnalyzedDataEvent, subscribers);
        }

        public void Unsubscribe(params Delegate[] unsubscribers)
        {
            EventExtensions.Unsubscribe(ref OnAudioAnalyzedDataEvent, unsubscribers);
        }

        public Delegate[] GetSubscribers()
        {
            return new Delegate[]
                   {
                       (AudioPlayerEvents.AudioClipChangedEvent) OnAudioClipChanged,
                       (DataProcessorsEvents.SpectrumProcessorDataEvent) SpectrumListenerDataReceived,
                       (GlobalAudioSettingsEvents.OnSpectrumAnalyzerSettingsEvent) OnAnalyzerSettingsChanged
                   };
        }
    }
}
