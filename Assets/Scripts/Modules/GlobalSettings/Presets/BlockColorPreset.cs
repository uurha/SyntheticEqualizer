using Modules.GlobalSettings.Model;
using UnityEngine;

namespace Modules.GlobalSettings.Presets
{
    [CreateAssetMenu(fileName = "BlockColors", menuName = "Presets/Block Colors", order = 0)]
    public class BlockColorPreset : ScriptableObject
    {
        [SerializeField] private ChunkUnitData highColor;
        [SerializeField] private ChunkUnitData middleColor;
        [SerializeField] private ChunkUnitData lowColor;

        public ChunkUnitsSettings GetSettings() =>
            new ChunkUnitsSettings(highColor, middleColor, lowColor);
    }
}
