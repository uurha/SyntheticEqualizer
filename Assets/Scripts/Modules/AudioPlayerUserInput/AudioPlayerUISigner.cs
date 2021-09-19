using System;
using System.Linq;
using Base;
using CorePlugin.Attributes.Headers;
using CorePlugin.Attributes.Validation;
using CorePlugin.Cross.Events.Interface;
using CorePlugin.Extensions;
using Modules.AudioPlayer.Model;
using Modules.AudioPlayer.SubSystems.PlayerStates;
using SubModules.UI;
using UnityEngine;

namespace Modules.AudioPlayerUserInput
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

        private event CrossEventsType.UpdatePlayerState UpdatePlayerState;
        private event CrossEventsType.AskPlaylistClip AskPlaylistClip;
        private event CrossEventsType.AskAudioPlayerData AskAudioPlayerData;

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
            var data = AskAudioPlayerData?.Invoke();
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
            UpdatePlayerState?.Invoke(new SwitchPlayState(AskPlaylistClip));
        }

        private void UpdateVolume(float value)
        {
            var playerState = new VolumeState(value);
            UpdatePlayerState?.Invoke(playerState);
        }

        public void InvokeEvents()
        {
        }

        public void Subscribe(Delegate[] subscribers)
        {
            AskPlaylistClip += subscribers.Combine<CrossEventsType.AskPlaylistClip>();
            UpdatePlayerState += subscribers.Combine<CrossEventsType.UpdatePlayerState>();
            AskAudioPlayerData += subscribers.Combine<CrossEventsType.AskAudioPlayerData>();
        }

        public void Unsubscribe(Delegate[] unsubscribers)
        {
            AskPlaylistClip -= unsubscribers.Combine<CrossEventsType.AskPlaylistClip>();
            UpdatePlayerState -= unsubscribers.Combine<CrossEventsType.UpdatePlayerState>();
            AskAudioPlayerData -= unsubscribers.Combine<CrossEventsType.AskAudioPlayerData>();
        }

        public Delegate[] GetSubscribers()
        {
            return new Delegate[]
                   {
                       (CrossEventsType.OnAudioPlayerStateEvent) PlayButtonTextUpdate,
                       (CrossEventsType.OnPlaybackTime01ChangedEvent) _playbackSlider.SetValueWithoutNotify
                   };
        }
    }
}
