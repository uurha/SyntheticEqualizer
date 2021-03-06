using Modules.AudioPlayerModule.Interfaces;

namespace Modules.AudioPlayerModule.Systems.PlayerStates
{
    public readonly struct SwitchPauseState : IPlayerState
    {
        public void Execute(IAudioPlayer audioPlayer)
        {
            if (audioPlayer.IsPaused)
                audioPlayer.UnPause();
            else
                audioPlayer.Pause();
        }
    }
}
