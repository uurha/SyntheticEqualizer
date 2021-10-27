using System;
using Modules.Carting.Interfaces;
using Modules.Grid.Model;
using Modules.Grid.Systems.CellEntity;
using UnityEngine;

namespace Modules.Carting.Systems.CartingRoad
{
    [RequireComponent(typeof(DefaultCellComponent))]
    public class DefaultCartingRoadComponent : MonoBehaviour, ICartingRoadComponent
    {
        [SerializeField] private RoadDirection inDirection;
        [SerializeField] private RoadDirection outDirection;
        
        public RoadDirection InDirection => inDirection;
        public RoadDirection OutDirection => outDirection;
    }
}
