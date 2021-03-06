using CorePlugin.Attributes.Editor;
using Modules.Grid.SubSystems;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(GridCullingMask))]
    public class GridCullingMaskEditor : ValidationAttributeEditor
    {
        private bool _showHandles;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();
            if (!GUILayout.Button("Toggle culling boundary handles")) return;
            _showHandles = !_showHandles;
            SceneView.RepaintAll();
        }

        public void OnSceneGUI()
        {
            if (!_showHandles) return;
            var startProperty = serializedObject.FindProperty("cullingStart");
            var endProperty = serializedObject.FindProperty("cullingEnd");
            var start = startProperty.vector3Value;
            var end = endProperty.vector3Value;
            var quaternion = Quaternion.identity;
            var newStart = Handles.PositionHandle(start, quaternion);
            var newEnd = Handles.PositionHandle(end, quaternion);
            var size = newEnd - newStart;
            var center = size / 2;
            var color = Handles.color;
            Handles.color = Color.yellow;
            Handles.DrawWireCube(center, size);
            Handles.color = color;

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Update culling mask");
                startProperty.vector3Value = newStart;
                endProperty.vector3Value = newEnd;
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
