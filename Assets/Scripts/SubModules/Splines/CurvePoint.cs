using System;
using UnityEngine;

namespace SubModules.Splines
{
    [Serializable]
    public readonly struct CurvePoint
    {
        public Vector3 Position { get; }

        public Vector3 Tangent { get; }

        public Vector3 Normal { get; }

        public CurvePoint(Vector3 position, Vector3 tangent, Vector3 normal)
        {
            Position = position;
            Tangent = tangent;
            Normal = normal;
        }
    }
}
