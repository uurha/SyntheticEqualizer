namespace Modules.AudioAnalyse.Model
{
    public struct BPMCalculatorOutput
    {
        public bool BPMChanged { get; }
        public int BPM { get; }
        public int AverageBPM { get; }
        
        public BPMCalculatorOutput(bool bpmChanged, int bpm, int averageBPM)
        {
            BPMChanged = bpmChanged;
            BPM = bpm;
            AverageBPM = averageBPM;
        }
    }
}
