using System;

namespace Modules.Audio.SubSystems.SpectrumAnalyzer
{
    /// <summary>
    /// Computing an array of online auto correlators
    /// </summary>
    public class AutoCorrelation
    {
        private readonly float _decay;
        private readonly float[] _delays;
        private readonly int _delLength;
        private readonly float[] _outputs;

        private readonly float[] _weight;
        private readonly float _weightMiddleBPM = 120f;
        private int _index;

        public AutoCorrelation(int len, float decay, float framePeriod, float bandwidth)
        {
            _decay = decay;
            _delLength = len;
            _delays = new float[_delLength];
            _outputs = new float[_delLength];
            _index = 0;

            // calculate a log-lag gaussian weighting function, to prefer tempi around 120 bpm
            _weight = new float[_delLength];

            for (var i = 0; i < _delLength; ++i)
            {
                var bpm = 60.0f / (framePeriod * i);

                // weighting is Gaussian on log-BPM axis, centered at weight middle BPM, SD = woctavewidth octaves
                _weight[i] =
                    (float) Math.Exp(-0.5f * Math.Pow(Math.Log(bpm / _weightMiddleBPM) / Math.Log(2.0f) / bandwidth,
                                                      2.0f));
            }
        }

        public void NewVal(float val)
        {
            _delays[_index] = val;

            // update running auto correlator values
            for (var i = 0; i < _delLength; ++i)
            {
                var delix = (_index - i + _delLength) % _delLength;
                _outputs[i] += (1 - _decay) * (_delays[_index] * _delays[delix] - _outputs[i]);
            }
            if (++_index == _delLength) _index = 0;
        }

        // read back the current auto correlator value at a particular lag
        public float AutoCorrelator(int del)
        {
            var correlated = _weight[del] * _outputs[del];
            return correlated;
        }
    }
}
