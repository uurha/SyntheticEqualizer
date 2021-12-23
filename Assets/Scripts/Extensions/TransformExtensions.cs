using Modules.Grid.Model;
using UnityEngine;

namespace Extensions
{
    public static class TransformExtensions
    {
        public static void SetOrientation(this Transform transform, Orientation orientation)
        {
            if (orientation.IsLocal)
            {
                transform.localPosition = orientation.Position;
                transform.localRotation = orientation.Rotation;
                transform.localScale = orientation.Scale;
            }
            else
            {
                transform.position = orientation.Position;
                transform.rotation = orientation.Rotation;
                transform.localScale = Vector3.one;

                //this simulates what it would be like to set lossyScale considering the way unity treats it
                var m = transform.worldToLocalMatrix;
                m.SetColumn(3, new Vector4(0f, 0f, 0f, 1f));
                transform.localScale = m.MultiplyPoint(orientation.Scale);
            }
        }

    }
}
