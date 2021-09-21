using System;
using Base.Deque;
using Modules.AudioLoader.Model;
using Modules.AudioPlayer.Model;
using Modules.AudioPlayer.SubSystems.Playlist;
using Modules.Grid.Model;
using UnityEngine;

namespace Base
{
    public static class CrossEventsType
    {
        public delegate void OnAudioAnalyzedDataUpdateEvent(float[] data);

        public delegate void OnAudioClipEndedEvent();

        public delegate void OnAudioLoadRequested(AudioLoaderSettings loaderSettings, Action<AudioDataProgress> action);

        public delegate void OnAudioPlayerStateEvent(AudioPlayerState state);

        public delegate void OnBeatDetectedEvent();

        public delegate void OnBPMChangedEvent(int bpm);

        public delegate void OnGridUpdatedEvent(Conveyor<GridConfiguration> gridConfigurations);

        public delegate void OnPlaybackTime01ChangedEvent(float data);

        public delegate void OnSpectrumListenerDataUpdateEvent(SpectrumListenerData listenerData);

        public delegate AudioPlayerData RequestAudioPlayerData();

        public delegate AudioClip RequestPlaylistClip(PlaylistDirection direction);

        public delegate void UpdatePlayerState(IPlayerState playerState);
    }
}
