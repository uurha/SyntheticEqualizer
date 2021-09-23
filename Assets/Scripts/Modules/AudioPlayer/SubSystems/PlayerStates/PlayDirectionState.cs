﻿using Base;
using Modules.AudioPlayer.Interfaces;
using Modules.AudioPlayer.SubSystems.Playlist;

namespace Modules.AudioPlayer.SubSystems.PlayerStates
{
    public struct PlayDirectionState : IPlayerState
    {
        private AudioPlayerEvents.RequestPlaylistClip RequestPlaylistClip;
        private PlaylistDirection _direction;

        public PlayDirectionState(AudioPlayerEvents.RequestPlaylistClip requestPlaylistClip, PlaylistDirection direction)
        {
            RequestPlaylistClip = requestPlaylistClip;
            _direction = direction;
        }

        public void Execute(IAudioPlayer audioPlayer)
        {
            var clip = RequestPlaylistClip?.Invoke(_direction);
            if (clip == null) return;
            audioPlayer.Play(clip);
        }
    }
}
