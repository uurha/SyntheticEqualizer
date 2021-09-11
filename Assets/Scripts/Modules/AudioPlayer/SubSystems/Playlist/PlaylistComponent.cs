using System;
using System.Collections.Generic;
using Base;
using Base.Deque;
using CorePlugin.Attributes.Headers;
using CorePlugin.Cross.Events.Interface;
using CorePlugin.ReferenceDistribution;
using Modules.AudioPlayer.Interfaces;
using UnityEngine;

namespace Modules.AudioPlayer.SubSystems.Playlist
{
    public class PlaylistComponent : MonoBehaviour, IEventSubscriber
    {
        [SettingsHeader]
        [SerializeField] private bool autoStartNextClip;

        private IAudioPlayer _audioPlayer;
        private Deque<AudioClip> _audioClips;

        private void Awake()
        {
            _audioClips = new Deque<AudioClip>();
        }

        private void Start()
        {
            _audioPlayer = ReferenceDistributor.GetInterfaceReference<IAudioPlayer>();
        }

        public void AddClip(AudioClip clip)
        {
            _audioClips.AddLast(clip);
        }

        private void AudioClipEnded()
        {
            if (!autoStartNextClip) return;
            PlayNext();
        }

        public IEnumerable<Delegate> GetSubscribers()
        {
            return new[] {(CrossEventsType.OnAudioClipEndedEvent) AudioClipEnded};
        }

        public void PlayNext()
        {
            if (_audioClips.IsEmpty) return;
            var clip = _audioClips.First;
            _audioClips.RemoveFirst();
            _audioClips.AddLast(clip);
            _audioPlayer.Play(clip);
        }

        public void PlayPrevious()
        {
            if (_audioClips.IsEmpty) return;
            var clip = _audioClips.Last;
            _audioClips.RemoveLast();
            _audioClips.AddFirst(clip);
            _audioPlayer.Play(clip);
        }
    }
}
