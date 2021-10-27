using Modules.AudioPlayer.Interfaces;

namespace Modules.AudioPlayer.Systems.PlayerStates
{
    public readonly struct Time01State : IPlayerState
    {
        private readonly float _time01;

        public Time01State(float time01)
        {
            _time01 = time01;
        }

        public void Execute(IAudioPlayer audioPlayer)
        {
            audioPlayer.Time01 = _time01;
        }
    }
}
