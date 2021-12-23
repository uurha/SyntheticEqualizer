using System;
using UnityEngine;

namespace Modules.Grid.Systems.ChunkEntity.Unit
{
    [Serializable]
    public struct UnitPerlinParams : IOrientationOffsetParams
    {
        [SerializeField] private Vector3 initialLocalOffset;
        [SerializeField] private float offsetStrength;
        [SerializeField] private float offsetHeight;

        public Vector3 InitialLocalOffset => initialLocalOffset;
        public bool IsValid => offsetHeight * offsetStrength != 0;

        public float OffsetStrength => offsetStrength;

        public float OffsetHeight => offsetHeight;
    }
}
