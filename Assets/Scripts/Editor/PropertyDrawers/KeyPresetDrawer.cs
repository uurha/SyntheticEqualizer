using Modules.InputManagement.Model;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(KeyPreset))]
    public class KeyPresetDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var bufferProperty = property.Copy();
            
            if (position.Contains(Event.current.mousePosition))
            {
                if (Event.current.isKey)
                {
                    if (bufferProperty.Next(true))
                    {
                        bufferProperty.intValue = (int) Event.current.keyCode;
                    }
                }
            }
            
            property = bufferProperty;

            label.tooltip = $"Press any key to set value";
            EditorGUI.PropertyField(position, property, label,true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property);
        }
    }
}
