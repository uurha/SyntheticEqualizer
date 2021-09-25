using System;
using System.Collections.Generic;

namespace Modules.AudioAnalyse.Model
{
    [Serializable]
    public struct SpectrumProcessorData
    {
        public int Frequency { get; }
        public int NumberOfSamples { get; }
        public int Channels { get; }
        public List<float[]> SpectrumData { get; }
        public List<float[]> RawSpectrumData { get; }

        public SpectrumProcessorData(SpectrumListenerData spectrumListenerData, List<float[]> spectrumData)
        {
            Frequency = spectrumListenerData.Frequency;
            NumberOfSamples = spectrumListenerData.NumberOfSamples;
            RawSpectrumData = spectrumListenerData.RawSpectrumData;
            Channels = spectrumListenerData.Channels;
            SpectrumData = spectrumData;
        }
    }
}
