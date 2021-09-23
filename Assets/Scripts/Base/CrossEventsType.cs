using System;
using System.Collections.Generic;
using Base.Deque;
using Modules.AudioLoader.Model;
using Modules.AudioPlayer.Model;
using Modules.AudioPlayer.SubSystems.Playlist;
using Modules.Grid.Model;
using UnityEngine;

namespace Base
{
    public static class AudioPlayerEvents
    {
        public delegate void OnAudioAnalyzedDataUpdateEvent(List<float[]> data);

        public delegate void OnAudioClipEndedEvent();

        public delegate void OnAudioLoadRequested(AudioLoaderSettings loaderSettings, Action<AudioDataProgress> action);

        public delegate void OnAudioPlayerStateEvent(AudioPlayerState state);

        public delegate void OnPlaybackTime01ChangedEvent(float data);

        public delegate void OnAudioClipChanged();

        public delegate AudioPlayerData RequestAudioPlayerData();

        public delegate AudioClip RequestPlaylistClip(PlaylistDirection direction);

        public delegate void UpdateAudioPlayerState(IPlayerState playerState);
    }

    public static class BeatDetectionEvents
    {
        public delegate void OnBeatDetectedEvent();
    }

    public static class CrossEvents
    {
        public delegate void OnGridUpdatedEvent(Conveyor<GridConfiguration> gridConfigurations);

        public delegate void OnSpectrumListenerDataUpdateEvent(SpectrumListenerData listenerData);

    }
}
