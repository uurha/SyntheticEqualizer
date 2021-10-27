using System;
using Base;
using CorePlugin.Attributes.Headers;
using CorePlugin.Attributes.Validation;
using CorePlugin.Cross.Events.Interface;
using CorePlugin.Extensions;
using Modules.AudioPlayer.Model;
using Modules.AudioPlayer.Systems;
using SubModules.UI;
using UnityEngine;

namespace Modules.AudioPlayerUI.Systems.Signers
{
    public class AudioPlayerUISigner : MonoBehaviour, IEventSubscriber, IEventHandler
    {
        [ReferencesHeader]
        [HasComponent(typeof(ISlider<float>))]
        [SerializeField] private GameObject playbackSlider;

        [HasComponent(typeof(IButton))]
        [SerializeField] private GameObject playButton;

        [HasComponent(typeof(ISlider<float>))]
        [SerializeField] private GameObject volumeSlider;

        private ISlider<float> _playbackSlider;
        private IButton _playButton;
        private ISlider<float> _volumeSlider;

        private event AudioPlayerEvents.RequestAudioPlayerData RequestAudioPlayerData;

        private void Awake()
        {
            _playButton = playButton.GetComponent<IButton>();
            _playbackSlider = playbackSlider.GetComponent<ISlider<float>>();
            _volumeSlider = volumeSlider.GetComponent<ISlider<float>>();
        }

        private void Start()
        {
            _playButton.OnClick +=  AudioPlayerStateCaller.SwitchPlay;
            _playbackSlider.OnValueChanged +=  AudioPlayerStateCaller.SetPlaybackTime01;
            _volumeSlider.OnValueChanged += OnVolumeSliderValueChanged;
        }

        private void OnVolumeSliderValueChanged(float value)
        {
            AudioPlayerStateCaller.UpdateVolume(value, false);
        }

        private void PlayButtonTextUpdate(AudioPlayerState state)
        {
            if (state.HasFlag(AudioPlayerState.Play) &&
                !state.HasFlag(AudioPlayerState.Pause))
                _playButton.Text = "Stop";
            else
                _playButton.Text = "Play";
        }

        public void InvokeEvents()
        {
        }

        public void Subscribe(params Delegate[] subscribers)
        {
            EventExtensions.Subscribe(ref RequestAudioPlayerData, subscribers);
        }

        public void Unsubscribe(params Delegate[] unsubscribers)
        {
            EventExtensions.Unsubscribe(ref RequestAudioPlayerData, unsubscribers);
        }

        public Delegate[] GetSubscribers()
        {
            return new Delegate[]
                   {
                       (AudioPlayerEvents.AudioPlayerStateEvent) PlayButtonTextUpdate,
                       (AudioPlayerEvents.PlaybackTime01ChangedEvent) _playbackSlider.SetValueWithoutNotify,
                       (AudioPlayerEvents.AudioPlayerVolumeEvent) _volumeSlider.SetValueWithoutNotify
                   };
        }
    }
}
