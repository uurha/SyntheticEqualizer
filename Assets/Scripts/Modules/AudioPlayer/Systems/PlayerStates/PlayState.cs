using Base;
using Modules.AudioPlayer.Interfaces;
using Modules.AudioPlayer.Systems.Playlist;

namespace Modules.AudioPlayer.Systems.PlayerStates
{
    public readonly struct PlayState : IPlayerState
    {
        private readonly AudioPlayerEvents.RequestPlaylistClip _requestPlaylistClip;

        public PlayState(AudioPlayerEvents.RequestPlaylistClip requestPlaylistClip)
        {
            _requestPlaylistClip = requestPlaylistClip;
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
            var clip = _requestPlaylistClip?.Invoke(PlaylistDirection.Next);
            if (clip == null) return;
            audioPlayer.Play(clip);
        }
    }
}
