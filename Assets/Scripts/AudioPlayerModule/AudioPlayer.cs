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
        None,
        Play,
        Stop,
        Pause
    }

    [RequireComponent(typeof(AudioSource))] [OneAndOnly]
    public class AudioPlayer : MonoBehaviour, IAudioPlayer, IEventHandler, IDistributingReference
    {
        [SerializeField] private AudioSource audioSource;

        private event CrossEventsType.OnAudioPlayerStateEvent OnAudioPlayerState;

        private AudioPlayerState _currentState;

        public AudioClip Clip => audioSource.clip;

        public AudioPlayerState CurrentState
        {
            get => _currentState;
            private set
            {
                _currentState = value;
                OnAudioPlayerState?.Invoke(_currentState);
            }
        }

        public event Action<float> OnPlaybackTimeChangedEvent;
        public event Action<float> OnPlaybackTime01ChangedEvent;

        public float Time
        {
            get => audioSource.time;
            set => audioSource.time = value;
        }

        public float Time01
        {
            get => Mathf.InverseLerp(0, audioSource.clip.length, audioSource.time);
            set => audioSource.time = Mathf.Lerp(0, audioSource.clip.length, value);
        }

        public bool IsPlaying => _currentState == AudioPlayerState.Play;

        public bool IsPaused => _currentState == AudioPlayerState.Pause;

        public void Pause()
        {
            audioSource.Pause();
            CurrentState = AudioPlayerState.Pause;
        }

        public void UpPause()
        {
            audioSource.UnPause();
            CurrentState = AudioPlayerState.Play;
        }

        public void Stop()
        {
            audioSource.Stop();
            CurrentState = AudioPlayerState.Stop;
        }

        public void Play()
        {
            audioSource.Play();
            CurrentState = AudioPlayerState.Play;
        }

        public void Play(AudioClip clip)
        {
            audioSource.PlayOneShot(clip);
            CurrentState = AudioPlayerState.Play;
        }

        private void Update()
        {
            if(!IsPlaying) return;
            OnPlaybackTimeChangedEvent?.Invoke(Time);
            OnPlaybackTime01ChangedEvent?.Invoke(Time01);
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
