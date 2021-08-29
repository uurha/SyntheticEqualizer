using System;
using System.Collections.Generic;
using System.Linq;
using AudioModule.AudioPlayerSystem;
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
        
        private void Start()
        {
            var audioPlayer = ReferenceDistributor.GetReference<AudioPlayer>();
            analyzer.InitializeAudio(audioPlayer);
        }

        private void OnPlayerStateChanged(AudioPlayerState state)
        {
            analyzer.ReinitializeAudio();
        }
        
        public void InvokeEvents()
        {
            
        }

        public void Subscribe(IEnumerable<Delegate> subscribers)
        {
            foreach (var onSpectrumUpdated in subscribers.OfType<CrossEventsType.OnSpectrumUpdatedEvent>())
            {
                analyzer.OnSpectrumEvent += new Action<float[]>(onSpectrumUpdated);
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
            foreach (var onSpectrumUpdated in unsubscribers.OfType<CrossEventsType.OnSpectrumUpdatedEvent>())
            {
                analyzer.OnSpectrumEvent -= new Action<float[]>(onSpectrumUpdated);
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
                       (CrossEventsType.OnAudioPlayerStateEvent) OnPlayerStateChanged
                   };
        }
    }
}
