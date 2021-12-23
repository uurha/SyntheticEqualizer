using System;
using System.Diagnostics.Contracts;
using CorePlugin.Attributes.EditorAddons;
using Modules.Grid.Model;
using Modules.Grid.Systems.ChunkEntity.Interfaces;
using UnityEngine;

namespace Modules.Grid.Systems.ChunkEntity.Unit
{
    [Serializable]
    public struct PositionLocalHeightOffset : IOrientationOffset
    {
        [Pure]
        public Orientation GetOffsetOrientation(Orientation orientation, IOrientationOffsetParams offsetParams)
        {
            var vector3 = orientation.Position;
            var buffer = Vector3.zero;
            if(offsetParams.IsValid && offsetParams is UnitPerlinParams perlinParams)
            {
                buffer = new Vector3(0f, PerlinNoise(vector3, perlinParams.OffsetStrength, perlinParams.OffsetHeight), 0f) + perlinParams.InitialLocalOffset;
            }

            return new Orientation(buffer, true);
        }

        private float PerlinNoise(Vector3 vector3, float offsetStrength, float offsetHeight)
        {
            return Mathf.PerlinNoise(vector3.x / offsetStrength, vector3.z / offsetStrength) * offsetHeight;
        }
    }
}
