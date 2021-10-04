using System.Collections.Generic;

namespace Modules.AudioAnalyse.Model
{
    public struct SpectrumAnalyzerOutput
    {
        public int Frequency { get; }
        public int NumberOfSamples { get; }
        public int Channels { get; }
        public List<float[]> SpectrumData { get; }
        public List<float[]> RawSpectrumData { get; }
        public List<float[]> MeanSpectrumData { get; }
        public List<float[]> PeakSpectrumData { get; }
        public List<float[]> LevelsSpectrumData { get; }

        public SpectrumAnalyzerOutput(SpectrumProcessorOutput spectrumListenerData, List<float[]> meanSpectrumData,
                                      List<float[]> peakSpectrumData, List<float[]> levelsSpectrumData)
        {
            Frequency = spectrumListenerData.Frequency;
            NumberOfSamples = spectrumListenerData.NumberOfSamples;
            Channels = spectrumListenerData.Channels;
            RawSpectrumData = spectrumListenerData.RawSpectrumData;
            SpectrumData = spectrumListenerData.SpectrumData;
            MeanSpectrumData = meanSpectrumData;
            PeakSpectrumData = peakSpectrumData;
            LevelsSpectrumData = levelsSpectrumData;
        }
    }
}
