using Modules.AudioPlayer.Interfaces;

namespace Modules.AudioPlayer.Systems.PlayerStates
{
    public readonly struct SwitchMuteState : IPlayerState
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
