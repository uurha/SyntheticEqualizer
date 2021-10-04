using System;
using Base.Deque;
using Modules.AudioAnalyse.Model;
using Modules.AudioLoader.Model;
using Modules.AudioPlayerModule.Interfaces;
using Modules.AudioPlayerModule.Model;
using Modules.AudioPlayerModule.Systems.Playlist;
using Modules.GlobalAudioSettings.Systems;
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
        public delegate void SpectrumAnalyzerDataEvent(SpectrumAnalyzerOutput data);
    }

    public static class BeatDetectionEvents
    {
        public delegate void BeatDetectorEvent(BeatDetectorOutput beatAnalyzeData);
    }

    public static class GlobalAudioSettingsEvents
    {
        public delegate void OnSpectrumAnalyzerSettingsEvent(SpectrumAnalyzerSettings analyzerSettings);

        public delegate void OnSpectrumListenerSettingsEvent(SpectrumListenerSettings analyzerSettings);
    }

    public static class GridEvents
    {
        public delegate void GridConfigurationChangedEvent(Conveyor<GridConfiguration> gridConfigurations);
    }

    public static class DataProcessorsEvents
    {
        public delegate void SpectrumListenerDataEvent(SpectrumListenerOutput listenerData);

        public delegate void SpectrumProcessorDataEvent(SpectrumProcessorOutput listenerData);
    }
}
