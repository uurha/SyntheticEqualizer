﻿using Base;
using Modules.AudioPlayerModule.Interfaces;

namespace Modules.AudioPlayerModule.Systems.PlayerStates
{
    public readonly struct SwitchPlayState : IPlayerState
    {
        private readonly AudioPlayerEvents.RequestPlaylistClip RequestPlaylistClip;

        public SwitchPlayState(AudioPlayerEvents.RequestPlaylistClip requestPlaylistClip)
        {
            RequestPlaylistClip = requestPlaylistClip;
        }

        public void Execute(IAudioPlayer audioPlayer)
        {
            IPlayerState playerState;

            if (audioPlayer.IsPlaying)
            {
                playerState = new StopState();
            }
            else
            {
                playerState = audioPlayer.IsPaused
                                  ? (IPlayerState) new SwitchPauseState()
                                  : (IPlayerState) new PlayState(RequestPlaylistClip);
            }
            playerState.Execute(audioPlayer);
        }
    }
}
