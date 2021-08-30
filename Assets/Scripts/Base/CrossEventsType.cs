using System;
using AudioModule.AudioPlayerSystem;
using Base.Deque;
using GridModule.Model;

namespace Base
{
    public static class CrossEventsType
    {
        public delegate void OnGridUpdatedEvent(Conveyor<GridConfiguration> gridConfigurations);
        
        public delegate void OnBeatDetectedEvent();
        public delegate void OnAudioPlayerStateEvent(AudioPlayerState state);
        public delegate void OnAudioAnalyzedDataUpdateEvent(float[] data);
        public delegate void OnSpectrumListenerDataUpdateEvent(SpectrumListenerData listenerData);
        public delegate void OnBPMChangedEvent(int bpm);
    }
}
