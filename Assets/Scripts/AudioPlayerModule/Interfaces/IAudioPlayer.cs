using System;
using CorePlugin.ReferenceDistribution.Interface;
using UnityEngine;

namespace AudioPlayerModule.Interfaces
{
    public interface IAudioPlayer : IDistributingReference
    {
        public AudioClip Clip { get; }

        public AudioPlayerState CurrentState { get; }

        public event Action<float> OnPlaybackTimeChangedEvent;
        public event Action<float> OnPlaybackTime01ChangedEvent;

        public float Time { get; set; }
        public float Time01 { get; set; }
        public float Volume { get; set; }
        public bool IsPlaying { get; }
        public bool IsPaused { get; }

        public void Pause();

        public void UpPause();

        public void Stop();

        public void Play();

        public void Play(AudioClip clip);
    }
}
