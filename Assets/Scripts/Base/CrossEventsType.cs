using Base.Deque;
using Modules.AudioPlayer;
using Modules.Grid.Model;
using UnityEngine;

namespace Base
{
    public static class CrossEventsType
    {
        public delegate void OnAudioAnalyzedDataUpdateEvent(float[] data);

        public delegate void OnAudioPlayerStateEvent(AudioPlayerState state);

        public delegate void OnBeatDetectedEvent();

        public delegate void OnBPMChangedEvent(int bpm);

        public delegate void OnGridUpdatedEvent(Conveyor<GridConfiguration> gridConfigurations);

        public delegate KeyCode OnKeyCodeEvent();

        public delegate void OnSpectrumListenerDataUpdateEvent(SpectrumListenerData listenerData);
    }
}
