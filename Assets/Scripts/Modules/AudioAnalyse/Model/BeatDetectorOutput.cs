namespace Modules.AudioAnalyse.Model
{
    public readonly struct BeatDetectorOutput
    {
        public float[] AvgSpectrum { get; }
        public float[] FreqSpectrum { get; }
        public bool IsBass { get; }
        public bool IsLow { get; }
        public BPMCalculatorOutput BPMCalculatorOutput { get; }

        public BeatDetectorOutput(float[] avgSpectrum, float[] freqSpectrum, bool isBass, bool isLow,
                                  BPMCalculatorOutput bpm)
        {
            AvgSpectrum = avgSpectrum;
            FreqSpectrum = freqSpectrum;
            IsBass = isBass;
            IsLow = isLow;
            BPMCalculatorOutput = bpm;
        }
    }
}
