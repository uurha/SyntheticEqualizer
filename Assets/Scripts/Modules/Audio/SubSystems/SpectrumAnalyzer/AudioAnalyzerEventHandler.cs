using System;
using System.Collections.Generic;
using System.Linq;
using Base;
using CorePlugin.Attributes.Headers;
using CorePlugin.Cross.Events.Interface;
using Modules.AudioPlayer.Model;
using UnityEngine;

namespace Modules.Audio.SubSystems.SpectrumAnalyzer
{
    [RequireComponent(typeof(AudioAnalyzer))]
    public class AudioAnalyzerEventHandler : MonoBehaviour, IEventHandler, IEventSubscriber
    {
        [ReferencesHeader]
        [SerializeField] private AudioAnalyzer analyzer;

        private void OnAudioClipChanged()
        {
            analyzer.Deconstruct();
        }

        private void SpectrumListenerDataReceived(SpectrumListenerData listenerData)
        {
            if (!analyzer.IsInitialized)
            {
                analyzer.InitializeAudio(listenerData);
                analyzer.OnSpectrumReceived(listenerData);
                return;
            }
            analyzer.OnSpectrumReceived(listenerData);
        }

        public void InvokeEvents()
        {
        }

        public void Subscribe(params Delegate[] subscribers)
        {
            foreach (var onAudioAnalyzedDataUpdated in
                subscribers.OfType<AudioPlayerEvents.OnAudioAnalyzedDataUpdateEvent>())
                analyzer.OnAudioAnalyzedDataUpdated += new Action<List<float[]>>(onAudioAnalyzedDataUpdated);

            foreach (var onBeatDetected in subscribers.OfType<BeatDetectionEvents.OnBeatDetectedEvent>())
                analyzer.OnBeatEvent += new Action(onBeatDetected);
        }

        public void Unsubscribe(params Delegate[] unsubscribers)
        {
            foreach (var onSpectrumUpdated in unsubscribers.OfType<AudioPlayerEvents.OnAudioAnalyzedDataUpdateEvent>())
                analyzer.OnAudioAnalyzedDataUpdated -= new Action<List<float[]>>(onSpectrumUpdated);

            foreach (var onBeatDetected in unsubscribers.OfType<BeatDetectionEvents.OnBeatDetectedEvent>())
                analyzer.OnBeatEvent -= new Action(onBeatDetected);
        }

        public Delegate[] GetSubscribers()
        {
            return new Delegate[]
                   {
                       (AudioPlayerEvents.OnAudioClipChanged) OnAudioClipChanged,
                       (CrossEvents.OnSpectrumListenerDataUpdateEvent) SpectrumListenerDataReceived
                   };
        }
    }
}
