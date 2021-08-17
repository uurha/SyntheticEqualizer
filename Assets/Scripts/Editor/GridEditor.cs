using CorePlugin.Attributes.Editor;
using Grid;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(GridInitializer))]
    public class GridEditor : ValidationAttributeEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        
            EditorGUILayout.Space();
            var obj = target as GridInitializer;
        
            if (GUILayout.Button("Initialize"))
            {
                if (obj is { }) obj.Initialize();
            }
        
            if (GUILayout.Button("Generate Next Grid"))
            {
                if (obj is { }) obj.GenerateNextGrid();
            }
        }
    }
}
