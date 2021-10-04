#region license

// Copyright 2021 - 2021 Arcueid Elizabeth D'athemon
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//     http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

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
