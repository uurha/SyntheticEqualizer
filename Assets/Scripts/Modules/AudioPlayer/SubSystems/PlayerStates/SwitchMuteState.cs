using Modules.AudioPlayer.Interfaces;

namespace Modules.AudioPlayer.SubSystems.PlayerStates
{
    public struct SwitchMuteState : IPlayerState
    {
        public void Execute(IAudioPlayer audioPlayer)
        {
            if (audioPlayer.IsMuted)
                audioPlayer.UnMute();
            else
                audioPlayer.Mute();
        }
    }
}
