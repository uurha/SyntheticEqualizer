using Extensions;

namespace Modules.GlobalSettings.Model
{
    public readonly struct SpectrumAnalyzerSettings
    {
        public AudioExtensions.BandType BandType { get; }
        public float Sensibility { get; }
        public float FallSpeed { get; }
        public bool IsValid { get; }

        public SpectrumAnalyzerSettings(AudioExtensions.BandType bandType, float sensibility, float fallSpeed)
        {
            BandType = bandType;
            Sensibility = sensibility;
            FallSpeed = fallSpeed;
            IsValid = true;
        }
    }
}
