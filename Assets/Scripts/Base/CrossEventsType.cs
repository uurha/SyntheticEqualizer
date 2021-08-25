using System;
using Base.Deque;
using Grid.Model;

namespace Base
{
    public static class CrossEventsType
    {
        public delegate void OnGridChanged(Conveyor<GridConfiguration> gridConfigurations);
        
        public delegate void AudioMeanLevelsUpdated(float[] gridConfigurations);
        public delegate void AudioPeakLevelsUpdated(float[] gridConfigurations);
        public delegate void AudioLevelsUpdated(float[] gridConfigurations);
    }
}
