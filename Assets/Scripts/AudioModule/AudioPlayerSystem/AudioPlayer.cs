using System;
using System.Collections.Generic;
using System.Linq;
using AudioModule.AudioPlayerSystem.Interfaces;
using Base;
using CorePlugin.Cross.Events.Interface;
using CorePlugin.ReferenceDistribution.Interface;
using UnityEngine;

namespace AudioModule.AudioPlayerSystem
{
    public enum AudioPlayerState
    {
        Play,
        Stop,
        Restart,
        ChangeClip
    }
    
    [RequireComponent(typeof(AudioSource))]
    public class AudioPlayer : MonoBehaviour, IAudioPlayer, IDistributingReference, IEventHandler
    {
        [SerializeField] private AudioSource audioSource;

        private event CrossEventsType.OnAudioPlayerStateEvent OnAudioPlayerState;
        
        public AudioClip Clip => audioSource.clip;

        public bool IsPlaying => audioSource.isPlaying;

        public void Pause()
        {
            audioSource.Pause();
        }

        public void UpPause()
        {
            audioSource.UnPause();
        }

        public void Stop()
        {
            audioSource.Stop();
            OnAudioPlayerState?.Invoke(AudioPlayerState.Stop);
        }

        public void Play()
        {
            audioSource.Play();
            OnAudioPlayerState?.Invoke(AudioPlayerState.Play);
        }

        public void Play(AudioClip clip)
        {
            audioSource.PlayOneShot(clip);
            OnAudioPlayerState?.Invoke(AudioPlayerState.ChangeClip);
        }

        public void GetSpectrumData(float[] spectrum, int channel, FFTWindow fftWindow)
        {
            audioSource.GetSpectrumData(spectrum, channel, fftWindow);
        }

        public void InvokeEvents()
        {
            
        }

        public void Subscribe(IEnumerable<Delegate> subscribers)
        {
            foreach (var clipChangedEvent in subscribers.OfType<CrossEventsType.OnAudioPlayerStateEvent>())
            {
                OnAudioPlayerState += clipChangedEvent;
            }
        }

        public void Unsubscribe(IEnumerable<Delegate> unsubscribers)
        {
            foreach (var clipChangedEvent in unsubscribers.OfType<CrossEventsType.OnAudioPlayerStateEvent>())
            {
                OnAudioPlayerState -= clipChangedEvent;
            }
        }
    }
}
