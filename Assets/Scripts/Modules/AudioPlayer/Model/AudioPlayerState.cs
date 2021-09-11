using System;

namespace Modules.AudioPlayer.Model
{
    [Flags]
    public enum AudioPlayerState
    {
        None = 0,
        Play = 1,
        Stop = 2,
        Pause = 4,
        Muted = 8
    }
}
