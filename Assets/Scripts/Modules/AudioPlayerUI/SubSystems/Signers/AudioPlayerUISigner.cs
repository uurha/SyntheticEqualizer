using System;
using Base;
using CorePlugin.Attributes.Headers;
using CorePlugin.Attributes.Validation;
using CorePlugin.Cross.Events.Interface;
using CorePlugin.Extensions;
using Modules.AudioPlayer.Model;
using Modules.AudioPlayer.SubSystems.PlayerStates;
using SubModules.UI;
using UnityEngine;

namespace Modules.AudioPlayerUI.SubSystems.Signers
{
    public class AudioPlayerUISigner : MonoBehaviour, IEventSubscriber, IEventHandler
    {
        [ReferencesHeader]
        [RequireInterface(typeof(ISlider<float>))]
        [SerializeField] private Component playbackSlider;

        [RequireInterface(typeof(IButton))]
        [SerializeField] private Component playButton;

        [RequireInterface(typeof(ISlider<float>))]
        [SerializeField] private Component volumeSlider;

        private ISlider<float> _playbackSlider;
        private IButton _playButton;
        private ISlider<float> _volumeSlider;

        private event AudioPlayerEvents.UpdateAudioPlayerState UpdatePlayerState;
        private event AudioPlayerEvents.RequestPlaylistClip RequestPlaylistClip;
        private event AudioPlayerEvents.RequestAudioPlayerData RequestAudioPlayerData;

        private void Awake()
        {
            _playButton = playButton.GetComponent<IButton>();
            _playbackSlider = playbackSlider.GetComponent<ISlider<float>>();
            _volumeSlider = volumeSlider.GetComponent<ISlider<float>>();
        }

        private void Start()
        {
            _playButton.OnClick += SwitchPlay;
            _playbackSlider.OnValueChanged += SetPlaybackTime01;
            _volumeSlider.OnValueChanged += UpdateVolume;
            var data = RequestAudioPlayerData?.Invoke();
            if (data.HasValue) SetVolume(data.Value.Volume);
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
            var playerState = new Time01State(time);
            UpdatePlayerState?.Invoke(playerState);
        }

        private void SetVolume(float volume)
        {
            _volumeSlider.SetValueWithoutNotify(volume);
        }

        private void SwitchPlay()
        {
            UpdatePlayerState?.Invoke(new SwitchPlayState(RequestPlaylistClip));
        }

        private void UpdateVolume(float value)
        {
            var playerState = new VolumeState(value);
            UpdatePlayerState?.Invoke(playerState);
        }

        public void InvokeEvents()
        {
        }

        public void Subscribe(params Delegate[] subscribers)
        {
            EventExtensions.Subscribe(ref RequestPlaylistClip, subscribers);
            EventExtensions.Subscribe(ref UpdatePlayerState, subscribers);
            EventExtensions.Subscribe(ref RequestAudioPlayerData, subscribers);
        }

        public void Unsubscribe(params Delegate[] unsubscribers)
        {
            EventExtensions.Unsubscribe(ref RequestPlaylistClip, unsubscribers);
            EventExtensions.Unsubscribe(ref UpdatePlayerState, unsubscribers);
            EventExtensions.Unsubscribe(ref RequestAudioPlayerData, unsubscribers);
        }

        public Delegate[] GetSubscribers()
        {
            return new Delegate[]
                   {
                       (AudioPlayerEvents.AudioPlayerStateEvent) PlayButtonTextUpdate,
                       (AudioPlayerEvents.PlaybackTime01ChangedEvent) _playbackSlider.SetValueWithoutNotify
                   };
        }
    }
}
