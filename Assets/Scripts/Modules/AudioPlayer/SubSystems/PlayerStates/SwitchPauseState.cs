using Modules.AudioPlayer.Interfaces;

namespace Modules.AudioPlayer.SubSystems.PlayerStates
{
    public struct SwitchPauseState : IPlayerState
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
