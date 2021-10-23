using System;
using Base;
using CorePlugin.Attributes.EditorAddons;
using CorePlugin.Attributes.Headers;
using CorePlugin.Attributes.Validation;
using CorePlugin.Cross.Events.Interface;
using CorePlugin.Extensions;
using Extensions;
using Modules.AudioPlayerModule.Interfaces;
using Modules.AudioPlayerModule.Model;
using Modules.AudioPlayerModule.Systems.Playlist;
using UnityEngine;

namespace Modules.AudioPlayerModule.Systems.AudioPlayerComponent
{
    [RequireComponent(typeof(AudioSource))] [OneAndOnly] [CoreManagerElement]
    public class AudioPlayer : MonoBehaviour, IAudioPlayer, IEventHandler, IEventSubscriber
    {
        [ReferencesHeader]
        [SerializeField] private AudioSource audioSource;

        [SettingsHeader]
        [SerializeField] private float initialVolume = 0.5f;

        private AudioPlayerState _currentState;

        private event AudioPlayerEvents.AudioPlayerStateEvent OnAudioPlayerState;
        private event AudioPlayerEvents.AudioPlayerVolumeEvent OnAudioPlayerVolume;
        private event AudioPlayerEvents.RequestPlaylistClip RequestPlaylistClip;
        private event AudioPlayerEvents.PlaybackTime01ChangedEvent OnPlaybackTime01ChangedEvent;
        private event AudioPlayerEvents.AudioClipChangedEvent OnAudioClipChanged;

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
            get => Clip != null ? Mathf.InverseLerp(0, audioSource.clip.length, audioSource.time) : 0;
            set => audioSource.time = Clip != null ? Mathf.Lerp(0, audioSource.clip.length, value) : 0;
        }

        public float Volume
        {
            get => audioSource.volume;
            set
            {
                var buffer = Mathf.Clamp01(value);
                audioSource.volume = buffer;
                OnAudioPlayerVolume?.Invoke(buffer);
            }
        }

        private void Update()
        {
            if (!IsPlaying) return;
            OnPlaybackTime01ChangedEvent?.Invoke(Time01);
            if (!WaitUntilEnd()) return;
            Play(RequestPlaylistClip?.Invoke(PlaylistDirection.Next));
        }

        private AudioPlayerData RequestAudioPlayerData()
        {
            return new AudioPlayerData(this);
        }

        private void SetPlayerState(IPlayerState playerState)
        {
            playerState.Execute(this);
        }

        private bool WaitUntilEnd()
        {
            return audioSource.time >= audioSource.clip.length;
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
            if (audioSource.clip == null) return;
            audioSource.time = 0f;
            audioSource.Play();
            if ((CurrentState & AudioPlayerState.Stop) != 0) OnAudioClipChanged?.Invoke();
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

        public void UnMute()
        {
            audioSource.mute = false;
        }

        public void UnPause()
        {
            audioSource.UnPause();
            CurrentState = CurrentState.Unset(AudioPlayerState.Pause);
        }

        public void InvokeEvents()
        {
            Stop();
            Volume = initialVolume;
        }

        public void Subscribe(params Delegate[] subscribers)
        {
            EventExtensions.Subscribe(ref OnAudioPlayerState, subscribers);
            EventExtensions.Subscribe(ref OnAudioClipChanged, subscribers);
            EventExtensions.Subscribe(ref OnPlaybackTime01ChangedEvent, subscribers);
            EventExtensions.Subscribe(ref RequestPlaylistClip, subscribers);
            EventExtensions.Subscribe(ref OnAudioPlayerVolume, subscribers);
        }

        public void Unsubscribe(params Delegate[] unsubscribers)
        {
            EventExtensions.Unsubscribe(ref OnAudioPlayerState, unsubscribers);
            EventExtensions.Unsubscribe(ref OnAudioClipChanged, unsubscribers);
            EventExtensions.Unsubscribe(ref OnPlaybackTime01ChangedEvent, unsubscribers);
            EventExtensions.Unsubscribe(ref RequestPlaylistClip, unsubscribers);
            EventExtensions.Unsubscribe(ref OnAudioPlayerVolume, unsubscribers);
        }

        public Delegate[] GetSubscribers()
        {
            return new Delegate[]
                   {
                       (AudioPlayerEvents.UpdateAudioPlayerState) SetPlayerState,
                       (AudioPlayerEvents.RequestAudioPlayerData) RequestAudioPlayerData
                   };
        }
    }
}
