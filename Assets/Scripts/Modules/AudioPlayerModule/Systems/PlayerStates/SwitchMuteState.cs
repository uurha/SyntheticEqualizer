using Modules.AudioPlayerModule.Interfaces;

namespace Modules.AudioPlayerModule.Systems.PlayerStates
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
