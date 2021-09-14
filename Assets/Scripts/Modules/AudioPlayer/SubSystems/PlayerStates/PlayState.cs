﻿using Base;
using Modules.AudioPlayer.Interfaces;
using Modules.AudioPlayer.SubSystems.Playlist;

namespace Modules.AudioPlayer.SubSystems.PlayerStates
{
    public struct PlayState : IPlayerState
    {
        private CrossEventsType.AskPlaylistClip AskPlaylistClip;

        public PlayState(CrossEventsType.AskPlaylistClip askPlaylistClip)
        {
            AskPlaylistClip = askPlaylistClip;
        }

        public void Execute(IAudioPlayer audioPlayer)
        {
            if (audioPlayer.Clip != null)
                audioPlayer.Play();
            else
                PlayNext(audioPlayer);
        }

        private void PlayNext(IAudioPlayer audioPlayer)
        {
            var clip = AskPlaylistClip?.Invoke(PlaylistDirection.Next);
            if (clip == null) return;
            audioPlayer.Play(clip);
        }
    }
}
