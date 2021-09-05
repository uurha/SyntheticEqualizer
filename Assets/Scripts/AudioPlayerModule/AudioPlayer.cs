using System;
using System.Collections.Generic;
using System.Linq;
using AudioPlayerModule.Interfaces;
using Base;
using CorePlugin.Attributes.Headers;
using CorePlugin.Attributes.Validation;
using CorePlugin.Cross.Events.Interface;
using CorePlugin.ReferenceDistribution.Interface;
using Extensions;
using UnityEngine;

namespace AudioPlayerModule
{
    [Flags]
    public enum AudioPlayerState
    {
        None,
        Play,
        Stop,
        Pause,
        Muted
    }
    
    [RequireComponent(typeof(AudioSource))] [OneAndOnly]
    public class AudioPlayer : MonoBehaviour, IAudioPlayer, IEventHandler
    {
        [ReferencesHeader]
        [SerializeField] private AudioSource audioSource;

        [SettingsHeader]
        [SerializeField] private float initialVolume = 0.5f;

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

        public float Volume
        {
            get => audioSource.volume;
            set => audioSource.volume = value;
        }

        public bool IsPlaying => _currentState == AudioPlayerState.Play;

        public bool IsPaused => _currentState.HasFlag(AudioPlayerState.Pause);

        public void Pause()
        {
            audioSource.Pause();
            CurrentState = CurrentState.Set(AudioPlayerState.Pause);
        }

        public void UpPause()
        {
            audioSource.UnPause();
            CurrentState = CurrentState.Unset(AudioPlayerState.Pause);
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
            audioSource.volume = initialVolume;
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
