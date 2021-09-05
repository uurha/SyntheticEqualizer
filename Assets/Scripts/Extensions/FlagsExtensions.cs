using System;
using AudioPlayerModule;

namespace Extensions
{
    public static class FlagsExtensions
    {
        public static AudioPlayerState Set(this AudioPlayerState flag, AudioPlayerState value)
        {
            return flag | value;
        }
        
        public static AudioPlayerState Unset(this AudioPlayerState flag, AudioPlayerState value)
        {
            return flag & ~value;
        }
    }
}
