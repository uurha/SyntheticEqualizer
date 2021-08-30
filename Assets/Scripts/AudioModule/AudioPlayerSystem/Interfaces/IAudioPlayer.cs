using UnityEngine;

namespace AudioModule.AudioPlayerSystem.Interfaces
{
    public interface IAudioPlayer
    {
        public AudioClip Clip { get; }
        
        public bool IsPlaying { get; }

        public void Pause();

        public void UpPause();

        public void Stop();

        public void Play();

        public void Play(AudioClip clip);
    }
}
