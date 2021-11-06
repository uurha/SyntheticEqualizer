using System.Linq;
using CorePlugin.Attributes.Editor;
using CorePlugin.Extensions;
using EditorDataStorage.Editor;
using SubModules.Splines;
using UnityEditor;
using UnityEngine;

namespace Editor.PropertyDrawers
{
    [CustomEditor(typeof(SplineCurve))]
    public class SplineCurveEditor : ValidationAttributeEditor
    {
        private SplineCurve _splineCurves;
        private bool _showNormals;
        private float _normalsLenght = 0.2f;
        private Color _normalsColor = Color.gray;

        private bool _showTangents;
        private float _tangentsLenght = 0.2f;
        private Color _tangentsColor = Color.red;

        private Color _lineColor = Color.white;

        private bool _showPreviewPoint;
        private float _previewPoint;
        private float _previewPointSize;
        private bool _showPreviewStyles = true;

        protected override void OnEnable()
        {
            base.OnEnable();
            SceneView.duringSceneGui += DuringSceneGui;
            _splineCurves = (SplineCurve)target;
            EditorData.GetData(this, nameof(_lineColor));
            EditorData.GetData(this, nameof(_tangentsColor));
            EditorData.GetData(this, nameof(_normalsColor));
            EditorData.GetData(this, nameof(_normalsLenght));
            EditorData.GetData(this, nameof(_tangentsLenght));
            EditorData.GetData(this, nameof(_showPreviewPoint));
            EditorData.GetData(this, nameof(_previewPoint));
            EditorData.GetData(this, nameof(_previewPointSize));
            EditorData.GetData(this, nameof(_showTangents));
            EditorData.GetData(this, nameof(_showNormals));
        }

        private void DuringSceneGui(SceneView obj)
        {
            var field = _splineCurves.GetControlPoints();
            var size = field.Length;
            var dirty = false;

            for (var i = 0; i < size; i++)
            {
                var item = field[i];
                field[i] = Handles.PositionHandle(item, Quaternion.identity);
                if (field[i] == item) continue;
                dirty = true;
            }
            var points = _splineCurves.GetPoints();

            for (var index = 1; index < points.Length; index++)
            {
                var prevCurvePoint = points[index - 1];
                var curvePoint = points[index];
                Handles.color = _lineColor;
                Handles.DrawLine(prevCurvePoint.position, curvePoint.position);
                DrawNormal(prevCurvePoint);
                DrawTangent(prevCurvePoint);

                if (_showPreviewPoint)
                {
                    Handles.color = Color.white;

                    Handles.SphereHandleCap(0, _splineCurves.GetPoint(_previewPoint).position, Quaternion.identity,
                                            _previewPointSize, EventType.Repaint);
                }
            }
            if (!dirty) return;
            _splineCurves.SetControlPoints(field);
            EditorUtility.SetDirty(target);
        }

        private void DrawTangent(CurvePoint prevCurvePoint)
        {
            if (_showTangents)
            {
                Handles.color = _tangentsColor;

                Handles.DrawLine(prevCurvePoint.position,
                                 prevCurvePoint.position + (prevCurvePoint.tangent * _tangentsLenght));
            }
        }

        private void DrawNormal(CurvePoint prevCurvePoint)
        {
            if (_showNormals)
            {
                Handles.color = _normalsColor;

                Handles.DrawLine(prevCurvePoint.position,
                                 prevCurvePoint.position + (prevCurvePoint.normal * _normalsLenght));
            }
        }

        public override void OnInspectorGUI()
        {
            var dirty = false;
            EditorGUI.BeginChangeCheck();
            base.OnInspectorGUI();

            if (EditorGUI.EndChangeCheck())
            {
                dirty = true;
            }
            var field = _splineCurves.GetControlPoints();

            if (GUILayout.Button("Add Position"))
            {
                field = field.Length > 0 ? field.Append(field[field.Length - 1]).ToArray() : new Vector3[1];
                _splineCurves.SetControlPoints(field);
            }

            _showPreviewStyles =
                EditorGUI.BeginFoldoutHeaderGroup(EditorGUILayout.GetControlRect(), _showPreviewStyles,
                                                  "Preview Style");

            if (_showPreviewStyles)
            {
                DrawColorField(ref _lineColor, nameof(_lineColor));
                DrawBoolField(ref _showNormals, nameof(_showNormals));

                if (_showNormals)
                {
                    DrawColorField(ref _normalsColor, nameof(_normalsColor));
                    DrawFloatField(ref _normalsLenght, nameof(_normalsLenght), 0f);
                }
                DrawBoolField(ref _showTangents, nameof(_showTangents));

                if (_showTangents)
                {
                    DrawColorField(ref _tangentsColor, nameof(_tangentsColor));
                    DrawFloatField(ref _tangentsLenght, nameof(_tangentsLenght), 0f);
                }
                DrawBoolField(ref _showPreviewPoint, nameof(_showPreviewPoint));

                if (_showPreviewPoint)
                {
                    DrawFloatRangeField(ref _previewPoint, nameof(_previewPoint), 0f, 1f);
                    DrawFloatRangeField(ref _previewPointSize, nameof(_previewPointSize), 0.1f, 1f);
                }
            }
            EditorGUI.EndFoldoutHeaderGroup();
            if (!dirty) return;
            _splineCurves.SetDirty();
            EditorUtility.SetDirty(target);
        }

        private void DrawColorField(ref Color color, string fieldName)
        {
            var buffer = EditorGUI.ColorField(EditorGUILayout.GetControlRect(),
                                              new GUIContent(fieldName.PrettyCamelCase()
                                                                      .ToTitleCase()), color);
            if (color.Equals(buffer)) return;
            color = buffer;
            EditorData.SetData(this, fieldName);
            SceneView.RepaintAll();
        }

        private void DrawFloatField(ref float value, string fieldName, float minValue = float.MinValue,
                                    float maxValue = float.MaxValue)
        {
            var buffer = EditorGUI.FloatField(EditorGUILayout.GetControlRect(),
                                              new GUIContent(fieldName.PrettyCamelCase()
                                                                      .ToTitleCase()), value);
            if (value.Equals(buffer)) return;
            if (buffer <= minValue) buffer = minValue;
            if (buffer >= maxValue) buffer = maxValue;
            value = buffer;
            EditorData.SetData(this, fieldName);
            SceneView.RepaintAll();
        }

        private void DrawFloatRangeField(ref float value, string fieldName, float minValue = float.MinValue,
                                         float maxValue = float.MaxValue)
        {
            var buffer = EditorGUI.Slider(EditorGUILayout.GetControlRect(),
                                          new GUIContent(fieldName.PrettyCamelCase()
                                                                  .ToTitleCase()), value, minValue, maxValue);
            if (value.Equals(buffer)) return;
            if (buffer <= minValue) buffer = minValue;
            if (buffer >= maxValue) buffer = maxValue;
            value = buffer;
            EditorData.SetData(this, fieldName);
            SceneView.RepaintAll();
        }

        private void DrawBoolField(ref bool value, string fieldName)
        {
            var buffer = EditorGUI.Toggle(EditorGUILayout.GetControlRect(),
                                          new GUIContent(fieldName.PrettyCamelCase()
                                                                  .ToTitleCase()), value);
            if (value.Equals(buffer)) return;
            value = buffer;
            EditorData.SetData(this, fieldName);
            SceneView.RepaintAll();
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= DuringSceneGui;
        }
    }
}
