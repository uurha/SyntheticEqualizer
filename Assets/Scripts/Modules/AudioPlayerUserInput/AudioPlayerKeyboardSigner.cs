﻿using System;
using System.Collections.Generic;
using System.Linq;
using Base;
using CorePlugin.Attributes.Headers;
using CorePlugin.Cross.Events.Interface;
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

        private event CrossEventsType.AskPlaylistClip AskPlaylistClip;
        private event CrossEventsType.UpdatePlayerState UpdatePlayerState;

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
            var playerState = new PlayState(AskPlaylistClip);
            UpdatePlayerState?.Invoke(playerState);
        }

        private void PlayNext()
        {
            var playerState = new PlayDirectionState(AskPlaylistClip, PlaylistDirection.Next);
            UpdatePlayerState?.Invoke(playerState);
        }

        private void PlayPrevious()
        {
            var playerState = new PlayDirectionState(AskPlaylistClip, PlaylistDirection.Previous);
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
            UpdatePlayerState?.Invoke(new SwitchPlayState(AskPlaylistClip));
        }

        public void InvokeEvents()
        {
        }

        public void Subscribe(IEnumerable<Delegate> subscribers)
        {
            foreach (var askPlaylistClip in subscribers.OfType<CrossEventsType.AskPlaylistClip>())
                AskPlaylistClip += askPlaylistClip;

            foreach (var updatePlayerState in subscribers.OfType<CrossEventsType.UpdatePlayerState>())
                UpdatePlayerState += updatePlayerState;
        }

        public void Unsubscribe(IEnumerable<Delegate> unsubscribers)
        {
            foreach (var askPlaylistClip in unsubscribers.OfType<CrossEventsType.AskPlaylistClip>())
                AskPlaylistClip -= askPlaylistClip;

            foreach (var updatePlayerState in unsubscribers.OfType<CrossEventsType.UpdatePlayerState>())
                UpdatePlayerState -= updatePlayerState;
        }
    }
}
