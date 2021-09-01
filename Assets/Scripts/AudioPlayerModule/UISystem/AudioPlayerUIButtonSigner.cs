using System;
using System.Collections.Generic;
using AudioPlayerModule.Interfaces;
using Base;
using CorePlugin.Cross.Events.Interface;
using CorePlugin.ReferenceDistribution;
using UIModule;
using UnityEngine;
using UnityEngine.UI;

namespace AudioPlayerModule.UISystem
{
    public class AudioPlayerUIButtonSigner : MonoBehaviour, IEventSubscriber
    {
        [SerializeField] private TextedButton playButton;
        [SerializeField] private Slider playbackSlider;

        private IAudioPlayer _audioPlayer;
    
        // Start is called before the first frame update
        private void Start()
        {
            _audioPlayer = ReferenceDistributor.GetReference<AudioPlayer>();
            playButton.onClick += PlayButtonBehaviour;
            playbackSlider.onValueChanged.AddListener(SetPlaybackTime01);
            _audioPlayer.OnPlaybackTime01ChangedEvent += playbackSlider.SetValueWithoutNotify;
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
            }
            else
            {
                if (_audioPlayer.IsPaused)
                    _audioPlayer.UpPause();
                else
                    _audioPlayer.Play();
            }
        }

        private void PlayButtonTextUpdate(AudioPlayerState state)
        {
            playButton.text = state switch
                            {
                                AudioPlayerState.Play => "Stop",
                                AudioPlayerState.Stop => "Play",
                                AudioPlayerState.Pause => "Play",
                                AudioPlayerState.None => "Play",
                                _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
                            };
        }

        public IEnumerable<Delegate> GetSubscribers()
        {
            return new Delegate[] {(CrossEventsType.OnAudioPlayerStateEvent) PlayButtonTextUpdate};
        }
    }
}
