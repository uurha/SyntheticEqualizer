using Base;
using Modules.AudioPlayer.Interfaces;
using Modules.AudioPlayer.SubSystems.Playlist;

namespace Modules.AudioPlayer.SubSystems.PlayerStates
{
    public struct PlayDirectionState : IPlayerState
    {
        private CrossEventsType.AskPlaylistClip AskPlaylistClip;
        private PlaylistDirection _direction;

        public PlayDirectionState(CrossEventsType.AskPlaylistClip askPlaylistClip, PlaylistDirection direction)
        {
            AskPlaylistClip = askPlaylistClip;
            _direction = direction;
        }

        public void Execute(IAudioPlayer audioPlayer)
        {
            var clip = AskPlaylistClip?.Invoke(_direction);
            if (clip == null) return;
            audioPlayer.Play(clip);
        }
    }
}
