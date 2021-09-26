using Base;
using Modules.AudioPlayerModule.Interfaces;
using Modules.AudioPlayerModule.Systems.Playlist;

namespace Modules.AudioPlayerModule.Systems.PlayerStates
{
    public struct PlayState : IPlayerState
    {
        private AudioPlayerEvents.RequestPlaylistClip RequestPlaylistClip;

        public PlayState(AudioPlayerEvents.RequestPlaylistClip requestPlaylistClip)
        {
            RequestPlaylistClip = requestPlaylistClip;
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
            var clip = RequestPlaylistClip?.Invoke(PlaylistDirection.Next);
            if (clip == null) return;
            audioPlayer.Play(clip);
        }
    }
}
