using System;
using UnityEngine;

//TODO: Create custom editor based on this behaviour
namespace Extensions
{
    [ExecuteInEditMode]
    public class TransformRestrictions : MonoBehaviour
    {
        #if UNITY_EDITOR
        [Flags]
        public enum Axis
        {
            None = 0,
            X = 1 << 0,
            Y = 1 << 1,
            Z = 1 << 2
        }

        [SerializeField] private Axis positionRestrictedAxis;
        [SerializeField] private Axis rotationRestrictedAxis;

        private Vector3 _storedPosition;
        private Quaternion _storedRotation;

        private void LateUpdate()
        {
            if (transform.hasChanged)
            {
                var thisTransform = transform;
                var position = thisTransform.position;
                position = Restore(position, _storedPosition, positionRestrictedAxis);
                thisTransform.position = position;
                var rotation = thisTransform.rotation;

                rotation = Quaternion.Euler(Restore(rotation.eulerAngles,
                                                    _storedRotation.eulerAngles, rotationRestrictedAxis));
                thisTransform.rotation = rotation;
                _storedPosition = position;
                _storedRotation = rotation;
                thisTransform.hasChanged = false;
            }
        }

        private Vector3 Restore(Vector3 current, Vector3 stored, Axis axis)
        {
            var restored = current;
            if (axis.HasFlag(Axis.X)) restored.x = stored.x;
            if (axis.HasFlag(Axis.Y)) restored.y = stored.y;
            if (axis.HasFlag(Axis.Z)) restored.z = stored.z;
            return restored;
        }
        #endif
    }
}
