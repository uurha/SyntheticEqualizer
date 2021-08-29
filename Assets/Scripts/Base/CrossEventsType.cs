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
        public delegate void OnSpectrumUpdatedEvent(float[] spectrum);
        public delegate void OnBPMChangedEvent(int bpm);
    }
}
