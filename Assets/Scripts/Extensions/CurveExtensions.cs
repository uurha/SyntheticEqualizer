using System;
using UnityEngine;

namespace Extensions
{
    public static class CurveExtensions
    {
        public static void Clear(this AnimationCurve curve)
        {
            curve.keys = Array.Empty<Keyframe>();
        }

        public static float[] CurveToArray(this AnimationCurve curve, int arrayLenght)
        {
            var array = new float[arrayLenght];

            for (var index = 0; index < arrayLenght; index++)
            {
                var t = Mathf.InverseLerp(0, arrayLenght, index);
                array[index] = curve.Evaluate(t);
            }
            return array;
        }
    }
}
