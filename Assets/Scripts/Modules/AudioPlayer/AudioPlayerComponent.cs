using System;
using System.Collections.Generic;
using System.Linq;
using Base;
using CorePlugin.Attributes.Headers;
using CorePlugin.Attributes.Validation;
using CorePlugin.Cross.Events.Interface;
using Extensions;
using Modules.AudioPlayer.Interfaces;
using Modules.AudioPlayer.Model;
using UnityEngine;

namespace Modules.AudioPlayer
{
    [RequireComponent(typeof(AudioSource))] [OneAndOnly]
    public class AudioPlayerComponent : MonoBehaviour, IAudioPlayer, IEventHandler
    {
        [ReferencesHeader]
        [SerializeField] private AudioSource audioSource;

        [SettingsHeader]
        [SerializeField] private float initialVolume = 0.5f;

        private AudioPlayerState _currentState;

        private event CrossEventsType.OnAudioPlayerStateEvent OnAudioPlayerState;
        private event CrossEventsType.OnAudioClipEndedEvent OnAudioClipEnded;

        public event Action<float> OnPlaybackTimeChangedEvent;
        public event Action<float> OnPlaybackTime01ChangedEvent;

        public AudioClip Clip => audioSource.clip;

        public bool IsPlaying => _currentState == AudioPlayerState.Play;
        public bool IsMuted => audioSource.mute;

        public bool IsPaused => _currentState.HasFlag(AudioPlayerState.Pause);

        public AudioPlayerState CurrentState
        {
            get => _currentState;
            private set
            {
                _currentState = value;
                OnAudioPlayerState?.Invoke(_currentState);
            }
        }

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

        private void Update()
        {
            if (!IsPlaying) return;
            OnPlaybackTimeChangedEvent?.Invoke(Time);
            OnPlaybackTime01ChangedEvent?.Invoke(Time01);

            if (WaitUntilEnd())
            {
                OnAudioClipEnded?.Invoke();
                Stop();
            }
        }

        public void InvokeEvents()
        {
            Stop();
            audioSource.volume = initialVolume;
        }

        public void Mute()
        {
            audioSource.mute = true;
        }

        public void Pause()
        {
            audioSource.Pause();
            CurrentState = CurrentState.Set(AudioPlayerState.Pause);
        }

        public void Play()
        {
            audioSource.Play();
            CurrentState = AudioPlayerState.Play;
        }

        public void Play(AudioClip clip)
        {
            Stop();
            audioSource.clip = clip;
            Play();
        }

        public void Stop()
        {
            audioSource.Stop();
            CurrentState = AudioPlayerState.Stop;
        }

        public void Subscribe(IEnumerable<Delegate> subscribers)
        {
            foreach (var audioPlayerStateEvent in subscribers.OfType<CrossEventsType.OnAudioPlayerStateEvent>())
                OnAudioPlayerState += audioPlayerStateEvent;

            foreach (var audioClipEnded in subscribers.OfType<CrossEventsType.OnAudioClipEndedEvent>())
                OnAudioClipEnded += audioClipEnded;
        }

        public void UnMute()
        {
            audioSource.mute = false;
        }

        public void UnPause()
        {
            audioSource.UnPause();
            CurrentState = CurrentState.Unset(AudioPlayerState.Pause);
        }

        public void Unsubscribe(IEnumerable<Delegate> unsubscribers)
        {
            foreach (var audioPlayerStateEvent in unsubscribers.OfType<CrossEventsType.OnAudioPlayerStateEvent>())
                OnAudioPlayerState -= audioPlayerStateEvent;

            foreach (var audioClipEnded in unsubscribers.OfType<CrossEventsType.OnAudioClipEndedEvent>())
                OnAudioClipEnded -= audioClipEnded;
        }

        private bool WaitUntilEnd()
        {
            return audioSource.time >= audioSource.clip.length;
        }
    }
}
