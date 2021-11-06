using System;
using UnityEngine;

namespace SubModules.Splines
{
    [Serializable]
    public class CurvePoint
    {
        public Vector3 position;
        public Vector3 tangent;
        public Vector3 normal;
    }
}
