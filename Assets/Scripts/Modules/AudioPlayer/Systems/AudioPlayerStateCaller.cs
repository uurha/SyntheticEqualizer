using System;
using Base;
using CorePlugin.Attributes.EditorAddons;
using CorePlugin.Cross.Events.Interface;
using CorePlugin.Extensions;
using CorePlugin.Singletons;
using Modules.AudioPlayer.Systems.PlayerStates;
using Modules.AudioPlayer.Systems.Playlist;
using Modules.InputManagement;
using UnityEngine;

namespace Modules.AudioPlayer.Systems
{
    [CoreManagerElement("Audio Player Controls")]
    public class AudioPlayerStateCaller : Singleton<AudioPlayerStateCaller>, IEventHandler
    {
        [SerializeField] private float sensivity = 0.08f;

        private event AudioPlayerEvents.RequestPlaylistClip RequestPlaylistClip;
        private event AudioPlayerEvents.UpdateAudioPlayerState UpdatePlayerState;

        private void Awake()
        {
            if (!IsInitialised)
            {
                _instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            InputHandler.OnMouseScrollDelta += OnMouseScrollDelta;
        }

        private void OnMouseScrollDelta(Vector2 value)
        {
            UpdateVolume(value.y * sensivity, true);
        }

        [EditorButton("Next", 1, 2)]
        public static void PlayNext()
        {
            if (!IsInitialised) return;

            _instance.UpdatePlayerState?.Invoke(new PlayDirectionState(_instance.RequestPlaylistClip,
                                                                       PlaylistDirection.Next));
        }

        [EditorButton("Previous", 1, 1)]
        public static void PlayPrevious()
        {
            if (!IsInitialised) return;

            _instance.UpdatePlayerState?.Invoke(new PlayDirectionState(_instance.RequestPlaylistClip,
                                                                       PlaylistDirection.Previous));
        }

        [EditorButton(2, 3)]
        public static void Stop()
        {
            if (!IsInitialised) return;
            _instance.UpdatePlayerState?.Invoke(new StopState());
        }

        [EditorButton("Mute", 4)]
        public static void SwitchMute()
        {
            if (!IsInitialised) return;
            _instance.UpdatePlayerState?.Invoke(new SwitchMuteState());
        }

        [EditorButton("Pause", 2, 2)]
        public static void SwitchPause()
        {
            if (!IsInitialised) return;
            _instance.UpdatePlayerState?.Invoke(new SwitchPauseState());
        }

        [EditorButton("Play", 2, 1)]
        public static void SwitchPlay()
        {
            if (!IsInitialised) return;
            _instance.UpdatePlayerState?.Invoke(new SwitchPlayState(_instance.RequestPlaylistClip));
        }

        public static void SetPlaybackTime01(float time)
        {
            if (!IsInitialised) return;
            _instance.UpdatePlayerState?.Invoke(new Time01State(time));
        }

        [EditorButton("Minimum volume", 3, 0.1f, false)]
        [EditorButton("Default volume", 3, 0.5f, false)]
        [EditorButton("Maximum volume", 3, 1.0f, false)]
        public static void UpdateVolume(float value, bool additive)
        {
            if (!IsInitialised) return;
            var playerState = new VolumeState(value, additive);
            _instance.UpdatePlayerState?.Invoke(playerState);
        }

        public void InvokeEvents()
        {
        }

        public void Subscribe(params Delegate[] subscribers)
        {
            EventExtensions.Subscribe(ref RequestPlaylistClip, subscribers);
            EventExtensions.Subscribe(ref UpdatePlayerState, subscribers);
        }

        public void Unsubscribe(params Delegate[] unsubscribers)
        {
            EventExtensions.Unsubscribe(ref RequestPlaylistClip, unsubscribers);
            EventExtensions.Unsubscribe(ref UpdatePlayerState, unsubscribers);
        }
    }
}
