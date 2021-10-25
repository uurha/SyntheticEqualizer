using Base;
using Modules.AudioPlayerModule.Interfaces;

namespace Modules.AudioPlayerModule.Systems.PlayerStates
{
    public readonly struct SwitchPlayState : IPlayerState
    {
        public readonly AudioPlayerEvents.RequestPlaylistClip _requestPlaylistClip;

        public SwitchPlayState(AudioPlayerEvents.RequestPlaylistClip requestPlaylistClip)
        {
            _requestPlaylistClip = requestPlaylistClip;
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
                if (audioPlayer.IsPaused)
                    playerState = new SwitchPauseState();
                else
                    playerState = new PlayState(_requestPlaylistClip);
            }
            playerState.Execute(audioPlayer);
        }
    }
}
