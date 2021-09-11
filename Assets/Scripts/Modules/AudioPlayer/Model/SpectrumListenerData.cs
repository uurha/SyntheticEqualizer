using System;

namespace Modules.AudioPlayer.Model
{
    [Serializable]
    public struct SpectrumListenerData
    {
        public int Frequency { get; }
        public int NumberOfSamples { get; }
        public int Channel { get; }
        public float[] SpectrumData { get; }

        public SpectrumListenerData(int frequency, int numberOfSamples, int channel, float[] spectrumData)
        {
            Frequency = frequency;
            NumberOfSamples = numberOfSamples;
            SpectrumData = spectrumData;
            Channel = channel;
        }
    }
}
