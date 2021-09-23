using System;
using Base;
using Base.Deque;
using CorePlugin.Cross.Events.Interface;
using UnityEngine;

namespace Modules.AudioPlayer.SubSystems.Playlist
{
    public enum PlaylistDirection
    {
        Previous,
        None,
        Next
    }

    public class PlaylistComponent : MonoBehaviour, IEventSubscriber
    {
        private Deque<AudioClip> _audioClips;

        private AudioClip _currentClip;

        private void Awake()
        {
            _audioClips = new Deque<AudioClip>();
        }

        public void AddClip(AudioClip clip)
        {
            _audioClips.AddLast(clip);
        }

        private AudioClip GetNextClip()
        {
            if (_audioClips.IsEmpty) return null;
            if (_currentClip != null) _audioClips.AddLast(_currentClip);
            var clip = _audioClips.First;
            _audioClips.RemoveFirst();
            _currentClip = clip;
            return clip;
        }

        private AudioClip GetPreviousClip()
        {
            if (_audioClips.IsEmpty) return null;
            if (_currentClip != null) _audioClips.AddFirst(_currentClip);
            var clip = _audioClips.Last;
            _audioClips.RemoveLast();
            _currentClip = clip;
            return clip;
        }

        private AudioClip RequestPlaylistClip(PlaylistDirection direction)
        {
            AudioClip clip = null;

            switch (direction)
            {
                case PlaylistDirection.Previous:
                    clip = GetPreviousClip();
                    break;
                case PlaylistDirection.None:
                    break;
                case PlaylistDirection.Next:
                    clip = GetNextClip();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
            return clip;
        }

        public Delegate[] GetSubscribers()
        {
            return new Delegate[] {(AudioPlayerEvents.RequestPlaylistClip) RequestPlaylistClip};
        }
    }
}
