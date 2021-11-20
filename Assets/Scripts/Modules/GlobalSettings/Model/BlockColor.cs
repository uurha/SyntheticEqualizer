using System;
using UnityEngine;

namespace Modules.GlobalSettings.Model
{
    [Serializable]
    public struct CellUnitData
    {
        [SerializeField] private Color maxColor;
        [SerializeField] private Color middleColor;
        [SerializeField] private Color lowestColor;
        [SerializeField] private float lowDelta;
        [SerializeField] private float middleDelta;

        public Color MaxColor => maxColor;
        public Color MiddleColor => middleColor;
        public Color LowestColor => lowestColor;
        public float LowDelta => lowDelta;
        public float MiddleDelta => middleDelta;
    }
}
