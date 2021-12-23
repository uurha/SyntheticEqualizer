using System;
using CorePlugin.Attributes.Headers;
using Modules.Carting.Interfaces;
using Modules.Grid.Model;
using Modules.Grid.Systems.ChunkEntity;
using SubModules.Splines;
using UnityEngine;

namespace Modules.Carting.Systems.CartingRoad
{
    [Serializable]
    public class DefaultCartingRoad : ICartingRoad
    {
        [ReferencesHeader]
        [SerializeField] private SplineCurve curve;
        [SettingsHeader]
        [SerializeField] private RoadDirection inDirection;
        [SerializeField] private RoadDirection outDirection;

        public RoadDirection InDirection => inDirection;
        public RoadDirection OutDirection => outDirection;

        public bool GetPoint(float passedPath, out CurvePoint vector3)
        {
            vector3 = default;
            if (curve == null) return false;
            vector3 = curve.GetPoint(passedPath, true);
            return passedPath <= 0.99f;
        }

        public void OnDisable()
        {
            
        }
    }
}
