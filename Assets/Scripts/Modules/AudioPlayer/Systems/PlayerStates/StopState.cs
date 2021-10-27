using Modules.AudioPlayer.Interfaces;

namespace Modules.AudioPlayer.Systems.PlayerStates
{
    public readonly struct StopState : IPlayerState
    {
        public void Execute(IAudioPlayer audioPlayer)
        {
            audioPlayer.Stop();
        }
    }
}
