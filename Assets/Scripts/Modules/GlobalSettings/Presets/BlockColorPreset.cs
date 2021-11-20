using Modules.GlobalSettings.Model;
using UnityEngine;

namespace Modules.GlobalSettings.Presets
{
    [CreateAssetMenu(fileName = "BlockColors", menuName = "Presets/Block Colors", order = 0)]
    public class BlockColorPreset : ScriptableObject
    {
        [SerializeField] private CellUnitData highColor;
        [SerializeField] private CellUnitData middleColor;
        [SerializeField] private CellUnitData lowColor;

        public CellUnitsSettings GetSettings() =>
            new CellUnitsSettings(highColor, middleColor, lowColor);
    }
}
