using Modules.Grid.Model;
using SubModules.Splines;
using UnityEngine;

namespace Modules.Carting.Interfaces
{
    public interface ICartingRoadComponent
    {
        public RoadDirection InDirection { get; }

        public RoadDirection OutDirection { get; }

        public bool GetPoint(float passedPath, out CurvePoint vector3);
    }
}
