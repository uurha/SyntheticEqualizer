using Modules.Grid.Model;
using UnityEngine;

namespace Modules.Carting.Interfaces
{
    public interface ICartingRoadComponent
    {
        public RoadDirection InDirection { get; }

        public RoadDirection OutDirection { get; }

        public bool GetPoint(float passedPath, out Vector3 vector3);
    }
}
