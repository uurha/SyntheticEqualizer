using System;
using System.Collections.Generic;
using AudioPlayerModule.Interfaces;
using Base;
using CorePlugin.Attributes.Headers;
using CorePlugin.Attributes.Validation;
using CorePlugin.Cross.Events.Interface;
using CorePlugin.ReferenceDistribution;
using UnityEngine;

namespace AudioPlayerModule.UISystem
{
    public class AudioPlayerUIButtonSigner : MonoBehaviour, IEventSubscriber
    {
        [ReferencesHeader]
        [RequireInterface(typeof(IButton))] [SerializeField] private Component playButton;
        [RequireInterface(typeof(ISlider<float>))] [SerializeField] private Component playbackSlider;
        [RequireInterface(typeof(ISlider<float>))] [SerializeField] private Component volumeSlider;

        private IAudioPlayer _audioPlayer;

        private IButton _playButton;
        private ISlider<float> _playbackSlider;
        private ISlider<float> _volumeSlider;

        private void Awake()
        {
            _playButton = playButton.GetComponent<IButton>();
            _playbackSlider = playbackSlider.GetComponent<ISlider<float>>();
            _volumeSlider = volumeSlider.GetComponent<ISlider<float>>();
        }

        private void Start()
        {
            _audioPlayer = ReferenceDistributor.GetInterfaceReference<IAudioPlayer>();
            
            _playButton.OnClick += PlayButtonBehaviour;
            
            _playbackSlider.OnValueChanged += SetPlaybackTime01;
            
            _audioPlayer.OnPlaybackTime01ChangedEvent += _playbackSlider.SetValueWithoutNotify;
            
            _volumeSlider.Value = _audioPlayer.Volume;
            _volumeSlider.OnValueChanged += SetVolume;
        }

        private void SetVolume(float value)
        {
            _audioPlayer.Volume = value;
        }

        private void SetPlaybackTime01(float time)
        {
            _audioPlayer.Time01 = time;
        }

        private void PlayButtonBehaviour()
        {
            if (_audioPlayer.IsPlaying)
            {
                _audioPlayer.Pause();
                return;
            }

            if (_audioPlayer.IsPaused)
                _audioPlayer.UpPause();
            else
                _audioPlayer.Play();
        }

        private void PlayButtonTextUpdate(AudioPlayerState state)
        {
            if (state.HasFlag(AudioPlayerState.Play) && !state.HasFlag(AudioPlayerState.Pause))
            {
                _playButton.Text = "Stop";
            }
            else
            {
                _playButton.Text = "Play";
            }
        }

        public IEnumerable<Delegate> GetSubscribers()
        {
            return new Delegate[] {(CrossEventsType.OnAudioPlayerStateEvent) PlayButtonTextUpdate};
        }
    }
}
