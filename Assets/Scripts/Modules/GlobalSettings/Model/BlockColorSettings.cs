using System;
using Extensions;
using Random = UnityEngine.Random;

namespace Modules.GlobalSettings.Model
{
    public readonly struct CellUnitsSettings
    {
        private readonly CellUnitData _frequencyLowColors;
        private readonly CellUnitData _frequencyMiddleColors;
        private readonly CellUnitData _frequencyHighColors;

        public CellUnitsSettings(CellUnitData highFrequencyColor, CellUnitData middleFrequencyColor,
                                  CellUnitData lowFrequencyColor)
        {
            _frequencyLowColors = lowFrequencyColor;
            _frequencyMiddleColors = middleFrequencyColor;
            _frequencyHighColors = highFrequencyColor;
        }

        public CellUnitData GetRandomData()
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
