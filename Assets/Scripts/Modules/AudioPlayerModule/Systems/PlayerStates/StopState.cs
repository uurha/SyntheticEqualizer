using Modules.AudioPlayerModule.Interfaces;

namespace Modules.AudioPlayerModule.Systems.PlayerStates
{
    public readonly struct StopState : IPlayerState
    {
        public void Execute(IAudioPlayer audioPlayer)
        {
            audioPlayer.Stop();
        }
    }
}
