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
        public List<float[]> SpectrumData { get; }

        public SpectrumListenerData(int frequency, int numberOfSamples, int channels, List<float[]> spectrumData)
        {
            Frequency = frequency;
            NumberOfSamples = numberOfSamples;
            SpectrumData = spectrumData;
            Channels = channels;
        }
    }
}
