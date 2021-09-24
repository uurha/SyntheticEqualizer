using System;
using System.Collections.Generic;

namespace Modules.AudioPlayer.Model
{
    [Serializable]
    public struct SpectrumListenerData
    {
        public int Frequency { get; }
        public int NumberOfSamples { get; }
        public int Channels { get; }
        public List<float[]> RawSpectrumData { get; }

        public SpectrumListenerData(int frequency, int numberOfSamples, int channels, List<float[]> rawSpectrumData)
        {
            Frequency = frequency;
            NumberOfSamples = numberOfSamples;
            RawSpectrumData = rawSpectrumData;
            Channels = channels;
        }
    }
}
