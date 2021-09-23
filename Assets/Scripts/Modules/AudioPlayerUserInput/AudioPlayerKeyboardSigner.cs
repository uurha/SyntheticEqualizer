using System;
using Base;
using CorePlugin.Attributes.Headers;
using CorePlugin.Cross.Events.Interface;
using CorePlugin.Extensions;
using Modules.AudioPlayer.Model;
using Modules.AudioPlayer.SubSystems.PlayerStates;
using Modules.AudioPlayer.SubSystems.Playlist;
using Modules.InputManagement;
using UnityEngine;

namespace Modules.AudioPlayerUserInput
{
    public class AudioPlayerKeyboardSigner : MonoBehaviour, IEventHandler
    {
        [SettingsHeader]
        [SerializeField] private PlaylistKeyboardSettings settings;

        private event AudioPlayerEvents.RequestPlaylistClip RequestPlaylistClip;
        private event AudioPlayerEvents.UpdateAudioPlayerState UpdatePlayerState;

        private void Start()
        {
            InputHandler.AddKeyEvent(settings.NextClipKey, PlayNext);
            InputHandler.AddKeyEvent(settings.PreviousClipKey, PlayPrevious);
            InputHandler.AddKeyEvent(settings.PlayClipKey, SwitchPlay);
            InputHandler.AddKeyEvent(settings.StopClipKey, Stop);
            InputHandler.AddKeyEvent(settings.PauseClipKey, SwitchPause);
            InputHandler.AddKeyEvent(settings.MuteClipKey, SwitchMute);
        }

        private void Play()
        {
            var playerState = new PlayState(RequestPlaylistClip);
            UpdatePlayerState?.Invoke(playerState);
        }

        private void PlayNext()
        {
            var playerState = new PlayDirectionState(RequestPlaylistClip, PlaylistDirection.Next);
            UpdatePlayerState?.Invoke(playerState);
        }

        private void PlayPrevious()
        {
            var playerState = new PlayDirectionState(RequestPlaylistClip, PlaylistDirection.Previous);
            UpdatePlayerState?.Invoke(playerState);
        }

        private void Stop()
        {
            UpdatePlayerState?.Invoke(new StopState());
        }

        private void SwitchMute()
        {
            UpdatePlayerState?.Invoke(new SwitchMuteState());
        }

        private void SwitchPause()
        {
            UpdatePlayerState?.Invoke(new SwitchPauseState());
        }

        private void SwitchPlay()
        {
            UpdatePlayerState?.Invoke(new SwitchPlayState(RequestPlaylistClip));
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
