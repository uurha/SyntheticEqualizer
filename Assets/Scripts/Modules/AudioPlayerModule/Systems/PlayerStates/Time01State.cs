using Modules.AudioPlayerModule.Interfaces;

namespace Modules.AudioPlayerModule.Systems.PlayerStates
{
    public struct Time01State : IPlayerState
    {
        private float _time01;

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
