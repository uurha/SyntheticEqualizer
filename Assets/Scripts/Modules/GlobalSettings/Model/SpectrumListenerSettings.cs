using UnityEngine;

namespace Modules.GlobalSettings.Model
{
    public readonly struct SpectrumListenerSettings
    {
        public FFTWindow FFTWindow { get; }
        public int NumberOfSamples { get; }
        public bool IsValid { get; }

        public SpectrumListenerSettings(FFTWindow fftWindow, int numberOfSamples)
        {
            FFTWindow = fftWindow;
            NumberOfSamples = numberOfSamples;
            IsValid = true;
        }
    }
}
