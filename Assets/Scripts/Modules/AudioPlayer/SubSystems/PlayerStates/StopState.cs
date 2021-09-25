using Modules.AudioPlayer.Interfaces;

namespace Modules.AudioPlayer.SubSystems.PlayerStates
{
    public struct StopState : IPlayerState
    {
        public void Execute(IAudioPlayer audioPlayer)
        {
            audioPlayer.Stop();
        }
    }
}
