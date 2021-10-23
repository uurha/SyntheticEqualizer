using Unity.Burst;
using UnityEngine;

namespace SubModules.Cell.Model
{
    [BurstCompile]
    public readonly struct Orientation
    {
        public Vector3 Position { get; }
        public Quaternion Rotation { get; }

        public Orientation(Transform transform, bool isLocal = false)
        {
            Position = isLocal ? transform.localPosition : transform.position;
            Rotation = isLocal ? transform.localRotation : transform.rotation;
        }

        public Orientation(Vector3 position, Quaternion rotation)
        {
            Position = position;
            Rotation = rotation;
        }

        public Orientation(Vector3 position)
        {
            Position = position;
            Rotation = Quaternion.identity;
        }

        public Orientation(Quaternion rotation)
        {
            Position = Vector3.zero;
            Rotation = rotation;
        }
    }
}
