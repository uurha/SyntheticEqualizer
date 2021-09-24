using System;
using System.Collections.Generic;
using Modules.AudioPlayer.Model;

namespace Modules.AudioAnalyse.Model
{
    [Serializable]
    public struct SpectrumAnalyzerData
    {
        public int Frequency { get; }
        public int NumberOfSamples { get; }
        public int Channels { get; }
        public List<float[]> RawSpectrumData { get; }
        public List<float[]> MeanSpectrumData { get; }
        public List<float[]> PeakSpectrumData { get; }
        public List<float[]> LevelsSpectrumData { get; }

        public SpectrumAnalyzerData(SpectrumListenerData spectrumListenerData, List<float[]> meanSpectrumData,
                                    List<float[]> peakSpectrumData, List<float[]> levelsSpectrumData)
        {
            Frequency = spectrumListenerData.Frequency;
            NumberOfSamples = spectrumListenerData.NumberOfSamples;
            Channels = spectrumListenerData.Channels;
            RawSpectrumData = spectrumListenerData.RawSpectrumData;
            MeanSpectrumData = meanSpectrumData;
            PeakSpectrumData = peakSpectrumData;
            LevelsSpectrumData = levelsSpectrumData;
        }
    }
}
