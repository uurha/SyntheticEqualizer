using Modules.AudioPlayer.Interfaces;

namespace Modules.AudioPlayer.Systems.PlayerStates
{
    public readonly struct VolumeState : IPlayerState
    {
        private readonly float _volume;
        private readonly bool _additive;

        public VolumeState(float volume, bool additive)
        {
            _additive = additive;
            _volume = volume;
        }

        public void Execute(IAudioPlayer audioPlayer)
        {
            if (_additive)
            {
                audioPlayer.Volume += _volume;
            }
            else
            {
                audioPlayer.Volume = _volume;
            }
        }
    }
}
