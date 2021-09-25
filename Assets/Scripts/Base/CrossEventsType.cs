using System;
using Base.Deque;
using Modules.AudioAnalyse.Model;
using Modules.AudioLoader.Model;
using Modules.AudioPlayer.Interfaces;
using Modules.AudioPlayer.Model;
using Modules.AudioPlayer.SubSystems.Playlist;
using Modules.Grid.Model;
using UnityEngine;

namespace Base
{
    public static class AudioPlayerEvents
    {
        public delegate void AudioClipChangedEvent();

        public delegate void AudioLoadRequested(AudioLoaderSettings loaderSettings, Action<AudioDataProgress> action);

        public delegate void AudioPlayerStateEvent(AudioPlayerState state);

        public delegate void PlaybackTime01ChangedEvent(float data);

        public delegate AudioPlayerData RequestAudioPlayerData();

        public delegate AudioClip RequestPlaylistClip(PlaylistDirection direction);

        public delegate void UpdateAudioPlayerState(IPlayerState playerState);
    }

    public static class AudioAnalyzerEvents
    {
        public delegate void SpectrumAnalyzerDataEvent(SpectrumAnalyzerData data);
    }

    public static class BeatDetectionEvents
    {
        public delegate void BeatDetectorEvent(BeatAnalyzeData beatAnalyzeData);
    }

    public static class GridEvents
    {
        public delegate void GridUpdatedEvent(Conveyor<GridConfiguration> gridConfigurations);
    }

    public static class DataProcessorsEvents
    {
        public delegate void SpectrumListenerDataEvent(SpectrumListenerData listenerData);

        public delegate void SpectrumProcessorDataEvent(SpectrumProcessorData listenerData);
    }
}
