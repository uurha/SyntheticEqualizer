using CorePlugin.Attributes.Validation;
using UnityEditor;
using UnityEngine;

namespace CorePlugin.Attributes.Editor.Drawers
{
    /// <summary>
    /// Drawer for the RequireInterface attribute.
    /// </summary>
    [CustomPropertyDrawer(typeof(RequireInterfaceAttribute))]
    public class RequireInterfaceDrawer : PropertyDrawer
    {
        /// <summary>
        /// Overrides GUI drawing for the attribute.
        /// </summary>
        /// <param name="position">Position.</param>
        /// <param name="property">Property.</param>
        /// <param name="label">Label.</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.ObjectReference)
            {
                var requiredAttribute = (RequireInterfaceAttribute) attribute;
                EditorGUI.BeginProperty(position, label, property);

                property.objectReferenceValue =
                    EditorGUI.ObjectField(position, label, property.objectReferenceValue,
                                          requiredAttribute.RequiredType, true);
                EditorGUI.EndProperty();
            }
            else
            {
                var previousColor = GUI.color;
                GUI.color = Color.red;
                EditorGUI.LabelField(position, label, new GUIContent("Property is not a reference type"));
                GUI.color = previousColor;
            }
        }
    }
}
