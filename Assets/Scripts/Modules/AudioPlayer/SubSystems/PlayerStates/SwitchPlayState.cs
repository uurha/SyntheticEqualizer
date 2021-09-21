using Base;
using Modules.AudioPlayer.Interfaces;

namespace Modules.AudioPlayer.SubSystems.PlayerStates
{
    public struct SwitchPlayState : IPlayerState
    {
        private CrossEventsType.RequestPlaylistClip RequestPlaylistClip;

        public SwitchPlayState(CrossEventsType.RequestPlaylistClip requestPlaylistClip)
        {
            RequestPlaylistClip = requestPlaylistClip;
        }

        public void Execute(IAudioPlayer audioPlayer)
        {
            IPlayerState playerState;

            if (audioPlayer.IsPlaying)
                playerState = new StopState();
            else
                playerState = new PlayState(RequestPlaylistClip);
            playerState.Execute(audioPlayer);
        }
    }

    public struct VolumeState : IPlayerState
    {
        private float _volume;

        public VolumeState(float volume)
        {
            _volume = volume;
        }

        public void Execute(IAudioPlayer audioPlayer)
        {
            audioPlayer.Volume = _volume;
        }
    }

    public struct Time01State : IPlayerState
    {
        private float _time01;

        public Time01State(float time01)
        {
            _time01 = time01;
        }

        public void Execute(IAudioPlayer audioPlayer)
        {
            audioPlayer.Time01 = _time01;
        }
    }
}
