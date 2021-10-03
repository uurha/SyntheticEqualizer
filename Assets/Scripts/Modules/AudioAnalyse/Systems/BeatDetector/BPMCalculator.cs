using Modules.AudioAnalyse.Model;
using UnityEngine;

namespace Modules.AudioAnalyse.Systems.BeatDetector
{
    public class BPMCalculator
    {
        private readonly float _analyzeTime;
        private float _currentTime;
        private int _beatCount;
        private int _bpm;
        private int _averageBPM;
        
        public BPMCalculator(float analyzeTime)
        {
            _analyzeTime = analyzeTime;
            _bpm = -1;
        }

        public void Update(float deltaTime, bool isBeat, out BPMCalculatorOutput bpmOutput)
        {
            bpmOutput = new BPMCalculatorOutput(false, _bpm, _averageBPM);
            if (_currentTime < _analyzeTime)
            {
                _currentTime += deltaTime;

                if (isBeat)
                {
                    _beatCount++;
                }
                return;
            }
            
            var bpm = Mathf.RoundToInt((_beatCount * 60f) / _currentTime);

            if (_bpm == -1)
                _averageBPM = bpm;
            else
                _averageBPM = (_averageBPM + bpm) / 2;
            
            var bpmChanged = _bpm != bpm;
            _bpm = bpm;
            
            bpmOutput = new BPMCalculatorOutput(bpmChanged, _bpm, _averageBPM);

            _beatCount = 0;
            _currentTime = 0f;
        }
    }
}
