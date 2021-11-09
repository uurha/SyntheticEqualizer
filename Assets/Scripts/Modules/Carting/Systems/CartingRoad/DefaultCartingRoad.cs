using System;
using CorePlugin.Attributes.Headers;
using Modules.Carting.Interfaces;
using Modules.Grid.Model;
using Modules.Grid.Systems.CellEntity;
using SubModules.Splines;
using UnityEngine;

namespace Modules.Carting.Systems.CartingRoad
{
    [RequireComponent(typeof(DefaultCellComponent))]
    public class DefaultCartingRoad : MonoBehaviour, ICartingRoadComponent
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
            vector3 = curve.GetPoint(passedPath, true);
            return passedPath <= 0.99f;
        }
    }
}
