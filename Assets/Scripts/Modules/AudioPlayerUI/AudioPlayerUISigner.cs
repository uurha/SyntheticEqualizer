using System;
using System.Collections.Generic;
using Base;
using CorePlugin.Attributes.Headers;
using CorePlugin.Attributes.Validation;
using CorePlugin.Cross.Events.Interface;
using CorePlugin.ReferenceDistribution;
using Modules.AudioPlayer.Interfaces;
using Modules.AudioPlayer.Model;
using SubModules.UI;
using UnityEngine;

namespace Modules.AudioPlayerUI
{
    public class AudioPlayerUISigner : MonoBehaviour, IEventSubscriber
    {
        [RequireInterface(typeof(ISlider<float>))] [SerializeField]
        private Component playbackSlider;

        [ReferencesHeader]
        [RequireInterface(typeof(IButton))] [SerializeField]
        private Component playButton;

        [RequireInterface(typeof(ISlider<float>))] [SerializeField]
        private Component volumeSlider;

        private IAudioPlayer _audioPlayer;
        private ISlider<float> _playbackSlider;

        private IButton _playButton;
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

        public IEnumerable<Delegate> GetSubscribers()
        {
            return new Delegate[] {(CrossEventsType.OnAudioPlayerStateEvent) PlayButtonTextUpdate};
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
            if (state.HasFlag(AudioPlayerState.Play) &&
                !state.HasFlag(AudioPlayerState.Pause))
                _playButton.Text = "Stop";
            else
                _playButton.Text = "Play";
        }

        private void SetPlaybackTime01(float time)
        {
            _audioPlayer.Time01 = time;
        }

        private void SetVolume(float value)
        {
            _audioPlayer.Volume = value;
        }
    }
}
