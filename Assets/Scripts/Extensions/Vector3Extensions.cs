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
    }
}