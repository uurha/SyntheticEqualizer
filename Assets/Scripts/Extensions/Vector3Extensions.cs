using System;
using UnityEngine;

namespace Extensions
{
    public static class Vector3Extensions
    {
        public static bool Between(this Vector3 comparer, Vector3 start, Vector3 end)
        {
            return comparer.x >= start.x && comparer.x <= end.x && comparer.z >= start.z && comparer.z <= end.z;
        }

        public static Vector2 Swap(this Vector2 vector2)
        {
            return new Vector2(vector2.y, vector2.x);
        }

        public static Vector3 TransformPoint(this Vector3 vector3, Transform transform)
        {
            return transform.TransformPoint(vector3);
        }
        
        public static Vector3 TransformPoint(this Vector3 vector3, MonoBehaviour monoBehaviour)
        {
            return monoBehaviour.transform.TransformPoint(vector3);
        }

        public static Vector3 OnlyOneAxis(this Vector3 vector3, Axis axis)
        {
            var newVector3 = axis switch
                             {
                                 Axis.X => new Vector3(vector3.x, 0, 0),
                                 Axis.Y => new Vector3(0, vector3.y, 0),
                                 Axis.Z => new Vector3(0, 0, vector3.z),
                                 _ => throw new ArgumentOutOfRangeException(nameof(axis), axis, null)
                             };
            return newVector3;
        }
    }
}
