using System;
using System.Collections.Generic;
using System.Linq;
using AudioPlayerModule.Interfaces;
using Base;
using CorePlugin.Attributes.Validation;
using CorePlugin.Cross.Events.Interface;
using CorePlugin.ReferenceDistribution.Interface;
using UnityEngine;

namespace AudioPlayerModule
{
    public enum AudioPlayerState
    {
        Play,
        Stop
    }

    [RequireComponent(typeof(AudioSource))][OneAndOnly]
    public class AudioPlayer : MonoBehaviour, IAudioPlayer, IEventHandler, IDistributingReference
    {
        [SerializeField] private AudioSource audioSource;

        private event CrossEventsType.OnAudioPlayerStateEvent OnAudioPlayerState;
        
        public AudioClip Clip => audioSource.clip;

        public bool IsPlaying => audioSource.isPlaying;

        public void Pause()
        {
            audioSource.Pause();
        }

        public void UpPause()
        {
            audioSource.UnPause();
        }

        public void Stop()
        {
            audioSource.Stop();
            OnAudioPlayerState?.Invoke(AudioPlayerState.Stop);
        }

        public void Play()
        {
            audioSource.Play();
            OnAudioPlayerState?.Invoke(AudioPlayerState.Play);
        }

        public void Play(AudioClip clip)
        {
            audioSource.PlayOneShot(clip);
            OnAudioPlayerState?.Invoke(AudioPlayerState.Play);
        }

        public void InvokeEvents()
        {
            Stop();
        }

        public void Subscribe(IEnumerable<Delegate> subscribers)
        {
            foreach (var clipChangedEvent in subscribers.OfType<CrossEventsType.OnAudioPlayerStateEvent>())
            {
                OnAudioPlayerState += clipChangedEvent;
            }
        }

        public void Unsubscribe(IEnumerable<Delegate> unsubscribers)
        {
            foreach (var clipChangedEvent in unsubscribers.OfType<CrossEventsType.OnAudioPlayerStateEvent>())
            {
                OnAudioPlayerState -= clipChangedEvent;
            }
        }
    }
}
