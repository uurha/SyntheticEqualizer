using System;
using Extensions;
using Random = UnityEngine.Random;

namespace Modules.GlobalSettings.Model
{
    public readonly struct ChunkUnitsSettings
    {
        private readonly ChunkUnitData _frequencyLowColors;
        private readonly ChunkUnitData _frequencyMiddleColors;
        private readonly ChunkUnitData _frequencyHighColors;

        public ChunkUnitsSettings(ChunkUnitData highFrequencyColor, ChunkUnitData middleFrequencyColor,
                                  ChunkUnitData lowFrequencyColor)
        {
            _frequencyLowColors = lowFrequencyColor;
            _frequencyMiddleColors = middleFrequencyColor;
            _frequencyHighColors = highFrequencyColor;
        }

        public ChunkUnitData GetRandomData()
        {
            var values = (FrequencyType[])Enum.GetValues(typeof(FrequencyType));

            return values[Random.Range(0, values.Length)] switch
                   {
                       FrequencyType.Low => _frequencyLowColors,
                       FrequencyType.Middle => _frequencyMiddleColors,
                       FrequencyType.High => _frequencyHighColors,
                       _ => throw new ArgumentOutOfRangeException()
                   };
        }
    }
}
