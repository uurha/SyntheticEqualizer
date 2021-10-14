using Modules.AudioPlayerModule.Interfaces;

namespace Modules.AudioPlayerModule.Systems.PlayerStates
{
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
}
