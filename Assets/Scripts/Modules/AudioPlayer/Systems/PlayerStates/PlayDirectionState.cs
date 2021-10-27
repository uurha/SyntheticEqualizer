using Base;
using Modules.AudioPlayer.Interfaces;
using Modules.AudioPlayer.Systems.Playlist;

namespace Modules.AudioPlayer.Systems.PlayerStates
{
    public readonly struct PlayDirectionState : IPlayerState
    {
        private readonly AudioPlayerEvents.RequestPlaylistClip _requestPlaylistClip;
        private readonly PlaylistDirection _direction;

        public PlayDirectionState(AudioPlayerEvents.RequestPlaylistClip requestPlaylistClip,
                                  PlaylistDirection direction)
        {
            _requestPlaylistClip = requestPlaylistClip;
            _direction = direction;
        }

        public void Execute(IAudioPlayer audioPlayer)
        {
            var clip = _requestPlaylistClip?.Invoke(_direction);
            if (clip == null) return;
            audioPlayer.Play(clip);
        }
    }
}
