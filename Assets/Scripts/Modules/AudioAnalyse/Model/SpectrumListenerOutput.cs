using System.Collections.Generic;

namespace Modules.AudioAnalyse.Model
{
    public struct SpectrumListenerOutput
    {
        public int Frequency { get; }
        public int NumberOfSamples { get; }
        public int Channels { get; }
        public List<float[]> RawSpectrumData { get; }

        public SpectrumListenerOutput(int frequency, int numberOfSamples, int channels, List<float[]> rawSpectrumData)
        {
            Frequency = frequency;
            NumberOfSamples = numberOfSamples;
            RawSpectrumData = rawSpectrumData;
            Channels = channels;
        }
    }
}
