using System;
using Base.Deque;
using Modules.AudioLoader.Model;
using Modules.AudioPlayer.Model;
using Modules.Grid.Model;

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

        public delegate void OnSpectrumListenerDataUpdateEvent(SpectrumListenerData listenerData);
    }
}
