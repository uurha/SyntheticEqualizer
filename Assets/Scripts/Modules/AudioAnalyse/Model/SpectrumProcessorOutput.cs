using System.Collections.Generic;

namespace Modules.AudioAnalyse.Model
{
    public struct SpectrumProcessorOutput
    {
        public int Frequency { get; }
        public int NumberOfSamples { get; }
        public int Channels { get; }
        public List<float[]> SpectrumData { get; }
        public List<float[]> RawSpectrumData { get; }

        public SpectrumProcessorOutput(SpectrumListenerOutput spectrumListenerData, List<float[]> spectrumData)
        {
            Frequency = spectrumListenerData.Frequency;
            NumberOfSamples = spectrumListenerData.NumberOfSamples;
            RawSpectrumData = spectrumListenerData.RawSpectrumData;
            Channels = spectrumListenerData.Channels;
            SpectrumData = spectrumData;
        }
    }
}
