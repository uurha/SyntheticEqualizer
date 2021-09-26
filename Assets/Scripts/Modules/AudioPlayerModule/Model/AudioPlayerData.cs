using Modules.AudioPlayerModule.Interfaces;
using UnityEngine;

namespace Modules.AudioPlayerModule.Model
{
    public struct AudioPlayerData
    {
        public float Volume { get; }

        public float Time01 { get; }

        public AudioClip AudioClip { get; }

        public bool IsValid { get; }

        public AudioPlayerData(IAudioPlayer audioPlayer)
        {
            Volume = audioPlayer.Volume;
            Time01 = audioPlayer.Time01;
            AudioClip = audioPlayer.Clip;
            IsValid = true;
        }
    }
}
