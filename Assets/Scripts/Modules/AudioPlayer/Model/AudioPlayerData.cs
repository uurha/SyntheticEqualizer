using Modules.AudioPlayer.Interfaces;
using UnityEngine;

namespace Modules.AudioPlayer.Model
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
