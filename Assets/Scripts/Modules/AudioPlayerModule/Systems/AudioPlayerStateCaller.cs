using System;
using Base;
using CorePlugin.Attributes.EditorAddons;
using CorePlugin.Core;
using CorePlugin.Cross.Events.Interface;
using CorePlugin.Extensions;
using CorePlugin.Singletons;
using Modules.AudioPlayerModule.Systems.PlayerStates;
using Modules.AudioPlayerModule.Systems.Playlist;
using UnityEngine;

namespace Modules.AudioPlayerModule.Systems
{
    [CoreManagerElement("Audio Player Controls")]
    public class AudioPlayerStateCaller : Singleton<AudioPlayerStateCaller>, IEventHandler
    {
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

        [EditorButton("Next", 1)]
        public static void PlayNext()
        {
            if (!IsInitialised) return;

            _instance.UpdatePlayerState?.Invoke(new PlayDirectionState(_instance.RequestPlaylistClip,
                                                                       PlaylistDirection.Next));
        }

        [EditorButton("Previous", 1)]
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

        [EditorButton("Minimum volume", 3, 0.1f)]
        [EditorButton("Default volume", 3, 0.5f)]
        [EditorButton("Maximum volume", 3, 1.0f)]
        public static void UpdateVolume(float value)
        {
            if (!IsInitialised) return;
            var playerState = new VolumeState(value);
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
