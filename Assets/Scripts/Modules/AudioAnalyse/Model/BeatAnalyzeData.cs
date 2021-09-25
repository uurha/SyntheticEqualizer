namespace Modules.AudioAnalyse.Model
{
    public class BeatAnalyzeData
    {
        /// <summary>
        /// Reference to the array containing average values for the sample amplitudes
        /// </summary>
        public float[] AvgSpectrum;

        /// <summary>
        /// Reference to the array containing current samples and amplitudes
        /// </summary>
        public float[] FreqSpectrum;

        /// <summary>
        /// Bool to check if current value is higher than average for bass frequencies
        /// </summary>
        public bool IsBass;

        /// <summary>
        /// Bool to check if current value is higher than average for low-mid frequencies
        /// </summary>
        public bool IsLow;
    }
}
