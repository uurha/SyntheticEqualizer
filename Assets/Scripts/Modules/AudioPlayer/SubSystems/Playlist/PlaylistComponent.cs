using System;
using System.Collections.Generic;
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

        private void Awake()
        {
            _audioClips = new Deque<AudioClip>();
        }

        public void AddClip(AudioClip clip)
        {
            _audioClips.AddLast(clip);
        }

        private AudioClip AskPlaylistClip(PlaylistDirection direction)
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

        private AudioClip GetNextClip()
        {
            if (_audioClips.IsEmpty) return null;
            var clip = _audioClips.First;
            _audioClips.RemoveFirst();
            _audioClips.AddLast(clip);
            return clip;
        }

        private AudioClip GetPreviousClip()
        {
            if (_audioClips.IsEmpty) return null;
            var clip = _audioClips.Last;
            _audioClips.RemoveLast();
            _audioClips.AddFirst(clip);
            return clip;
        }

        public IEnumerable<Delegate> GetSubscribers()
        {
            return new[] {(CrossEventsType.AskPlaylistClip) AskPlaylistClip};
        }
    }
}
