using System;
using System.Collections.Generic;
using System.Linq;
using Base;
using CorePlugin.Cross.Events.Interface;
using CorePlugin.ReferenceDistribution;
using CorePlugin.ReferenceDistribution.Interface;
using Modules.AudioPlayer.Interfaces;
using Modules.AudioPlayer.SubSystems.Playlist;
using UnityEngine;

namespace Modules.AudioPlayer.SubSystems.ExternActions
{
    public class AudioPlayerExternAction : MonoBehaviour, IEventHandler, IDistributingReference
    {
        private IAudioPlayer _audioPlayer;

        private event CrossEventsType.AskPlaylistClip AskPlaylistClip;

        private void Start()
        {
            _audioPlayer = ReferenceDistributor.GetInterfaceReference<IAudioPlayer>();
        }

        public void InvokeEvents()
        {
        }

        private void Play()
        {
            if (_audioPlayer.Clip != null)
                _audioPlayer.Play();
            else
                PlayNext();
        }

        public void PlayNext()
        {
            var clip = AskPlaylistClip?.Invoke(PlaylistDirection.Next);
            if (clip == null) return;
            _audioPlayer.Play(clip);
        }

        public void PlayPrevious()
        {
            var clip = AskPlaylistClip?.Invoke(PlaylistDirection.Previous);
            if (clip == null) return;
            _audioPlayer.Play(clip);
        }

        public void Stop()
        {
            _audioPlayer.Stop();
        }

        public void Subscribe(IEnumerable<Delegate> subscribers)
        {
            foreach (var askPlaylistClip in subscribers.OfType<CrossEventsType.AskPlaylistClip>())
                AskPlaylistClip += askPlaylistClip;
        }

        public void SwitchMute()
        {
            if (_audioPlayer.IsMuted)
                _audioPlayer.UnMute();
            else
                _audioPlayer.Mute();
        }

        public void SwitchPause()
        {
            if (_audioPlayer.IsPaused)
                _audioPlayer.UnPause();
            else
                _audioPlayer.Pause();
        }

        public void SwitchPlay()
        {
            if (_audioPlayer.IsPlaying)
                Stop();
            else
                Play();
        }

        public void Unsubscribe(IEnumerable<Delegate> unsubscribers)
        {
            foreach (var askPlaylistClip in unsubscribers.OfType<CrossEventsType.AskPlaylistClip>())
                AskPlaylistClip -= askPlaylistClip;
        }
    }
}
