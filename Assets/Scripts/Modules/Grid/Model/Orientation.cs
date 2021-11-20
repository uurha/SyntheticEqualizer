using Unity.Burst;
using UnityEngine;

namespace Modules.Grid.Model
{
    [BurstCompile]
    public readonly struct Orientation
    {
        public bool IsLocal { get; }
        public Vector3 Position { get; }
        public Quaternion Rotation { get; }
        public Vector3 Scale { get; }

        public Orientation(Transform transform, bool isLocal = false)
        {
            IsLocal = isLocal;
            Position = isLocal ? transform.localPosition : transform.position;
            Rotation = isLocal ? transform.localRotation : transform.rotation;
            Scale = isLocal ? transform.localScale : transform.lossyScale;
        }

        public Orientation(Vector3 position, Quaternion rotation, Vector3 scale, bool isLocal = false)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale;
            IsLocal = isLocal;
        }

        public Orientation(Vector3 position, bool isLocal = false)
        {
            Position = position;
            Scale = Vector3.zero;
            Rotation = Quaternion.identity;
            IsLocal = isLocal;
        }

        public Orientation(Quaternion rotation, bool isLocal = false)
        {
            Position = Vector3.zero;
            Rotation = rotation;
            Scale = Vector3.zero;
            IsLocal = isLocal;
        }
    }
}
