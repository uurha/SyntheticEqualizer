using Modules.AudioPlayerModule.Model;
using UnityEngine;

namespace Modules.AudioPlayerModule.Interfaces
{
    public interface IAudioPlayer
    {
        public AudioClip Clip { get; }

        public AudioPlayerState CurrentState { get; }

        public float Time { get; set; }
        public float Time01 { get; set; }
        public float Volume { get; set; }
        public bool IsPlaying { get; }
        public bool IsPaused { get; }
        public bool IsMuted { get; }

        public void Pause();

        public void UnPause();

        public void Stop();

        public void Play();

        public void Mute();

        public void UnMute();

        public void Play(AudioClip clip);
    }
}
