using Base;
using Modules.AudioPlayerModule.Interfaces;
using Modules.AudioPlayerModule.Systems.Playlist;

namespace Modules.AudioPlayerModule.Systems.PlayerStates
{
    public readonly struct PlayDirectionState : IPlayerState
    {
        private readonly AudioPlayerEvents.RequestPlaylistClip RequestPlaylistClip;
        private readonly PlaylistDirection _direction;

        public PlayDirectionState(AudioPlayerEvents.RequestPlaylistClip requestPlaylistClip,
                                  PlaylistDirection direction)
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
