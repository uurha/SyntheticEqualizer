﻿using Modules.Grid.Model;
using SubModules.Splines;

namespace Modules.Carting.Interfaces
{
    public interface ICartingRoadComponent
    {
        public RoadDirection InDirection { get; }

        public RoadDirection OutDirection { get; }

        public bool GetPoint(float passedPath, out CurvePoint vector3);
    }
}
