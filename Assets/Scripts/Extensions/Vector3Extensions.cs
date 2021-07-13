using System.Linq;
using Grid;
using UnityEngine;

namespace Extensions
{
    public static class Vector3Extensions
    {
        public static bool Between(this Vector3 comparer, Vector3 start, Vector3 end)
        {
            return comparer.x >= start.x && comparer.x <= end.x && comparer.z >= start.z && comparer.z <= end.z;
        }
    }
}