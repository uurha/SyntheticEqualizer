using System;
using System.Linq;
using UnityEngine;

namespace Grid
{
    [Serializable]
    public struct Orientation
    {
        public Vector3 Position { get; }
        public Quaternion Rotation { get; }

        public Orientation(Transform transform)
        {
            Position = transform.position;
            Rotation = transform.rotation;
        }
    }
}
