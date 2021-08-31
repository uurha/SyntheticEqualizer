using System;
using System.Collections.Generic;
using AudioPlayerModule.Interfaces;
using Base;
using CorePlugin.Cross.Events.Interface;
using CorePlugin.ReferenceDistribution;
using UIModule;
using UnityEngine;

namespace AudioPlayerModule.UISystem
{
    public class AudioPlayerUIButtonSigner : MonoBehaviour, IEventSubscriber
    {
        [SerializeField] private TextedButton playButton;

        private IAudioPlayer _audioPlayer;
    
        // Start is called before the first frame update
        private void Start()
        {
            _audioPlayer = ReferenceDistributor.GetReference<AudioPlayer>();
            playButton.onClick += PlayButtonBehaviour;
        }

        private void PlayButtonBehaviour()
        {
            if (_audioPlayer.IsPlaying)
            {
                _audioPlayer.Stop();
            }
            else
            {
                _audioPlayer.Play();
            }
        }

        private void PlayButtonTextUpdate(AudioPlayerState state)
        {
            playButton.text = state switch
                            {
                                AudioPlayerState.Play => "Stop",
                                AudioPlayerState.Stop => "Play",
                                _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
                            };
        }

        public IEnumerable<Delegate> GetSubscribers()
        {
            return new Delegate[] {(CrossEventsType.OnAudioPlayerStateEvent) PlayButtonTextUpdate};
        }
    }
}
