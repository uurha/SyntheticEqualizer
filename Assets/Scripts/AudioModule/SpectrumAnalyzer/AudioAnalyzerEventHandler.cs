using System;
using System.Collections.Generic;
using System.Linq;
using AudioPlayerModule;
using Base;
using CorePlugin.Attributes.Headers;
using CorePlugin.Cross.Events.Interface;
using CorePlugin.ReferenceDistribution;
using UnityEngine;

namespace AudioModule.SpectrumAnalyzer
{
    [RequireComponent(typeof(AudioAnalyzer))]
    public class AudioAnalyzerEventHandler : MonoBehaviour, IEventHandler, IEventSubscriber
    {
        [ReferencesHeader]
        [SerializeField] private AudioAnalyzer analyzer;
        
        private void OnPlayerStateChanged(AudioPlayerState state)
        {
            switch (state)
            {
                case AudioPlayerState.Play:
                    analyzer.SetStateAnalyzing(true);
                    break;
                case AudioPlayerState.Stop:
                    analyzer.SetStateAnalyzing(false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private void SpectrumListenerDataReceived(SpectrumListenerData listenerData)
        {
            if (!analyzer.IsInitialized)
            {
                analyzer.InitializeAudio(listenerData.Frequency, listenerData.NumberOfSamples);
                analyzer.OnSpectrumReceived(listenerData.SpectrumData);
                return;
            }
            
            analyzer.OnSpectrumReceived(listenerData.SpectrumData);
        }
        
        public void InvokeEvents()
        {
            
        }

        public void Subscribe(IEnumerable<Delegate> subscribers)
        {
            foreach (var onAudioAnalyzedDataUpdated in subscribers.OfType<CrossEventsType.OnAudioAnalyzedDataUpdateEvent>())
            {
                analyzer.OnAudioAnalyzedDataUpdated += new Action<float[]>(onAudioAnalyzedDataUpdated);
            }
            
            foreach (var onBeatDetected in subscribers.OfType<CrossEventsType.OnBeatDetectedEvent>())
            {
                analyzer.OnBeatEvent += new Action(onBeatDetected);
            }
            
            foreach (var onBPMChanged in subscribers.OfType<CrossEventsType.OnBPMChangedEvent>())
            {
                analyzer.OnBPMChanged += new Action<int>(onBPMChanged);
            }
        }

        public void Unsubscribe(IEnumerable<Delegate> unsubscribers)
        {
            foreach (var onSpectrumUpdated in unsubscribers.OfType<CrossEventsType.OnAudioAnalyzedDataUpdateEvent>())
            {
                analyzer.OnAudioAnalyzedDataUpdated -= new Action<float[]>(onSpectrumUpdated);
            }
            
            foreach (var onBeatDetected in unsubscribers.OfType<CrossEventsType.OnBeatDetectedEvent>())
            {
                analyzer.OnBeatEvent -= new Action(onBeatDetected);
            }
            
            foreach (var onBPMChanged in unsubscribers.OfType<CrossEventsType.OnBPMChangedEvent>())
            {
                analyzer.OnBPMChanged -= new Action<int>(onBPMChanged);
            }
        }

        public IEnumerable<Delegate> GetSubscribers()
        {
            return new Delegate[]
                   {
                       (CrossEventsType.OnAudioPlayerStateEvent) OnPlayerStateChanged,
                       (CrossEventsType.OnSpectrumListenerDataUpdateEvent)SpectrumListenerDataReceived
                   };
        }
    }
}
