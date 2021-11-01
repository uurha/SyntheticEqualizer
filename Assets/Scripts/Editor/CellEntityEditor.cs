using System;
using CorePlugin.Attributes.Editor;
using Modules.Carting.Interfaces;
using Modules.Grid.Interfaces;
using Modules.Grid.Model;
using Modules.Grid.Systems.CellEntity;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(DefaultCellComponent))]
    public class CellEntityEditor : ValidationAttributeEditor
    {
        private readonly float size = 1f;
        private ICartingRoadComponent _cartingRoadComponent;
        private ICellComponent _cellEntity;
        private bool _isRoadComponentAttached;

        protected override void OnEnable()
        {
            base.OnEnable();
            var cell = target as DefaultCellComponent;
            if(cell == null) return;
            _isRoadComponentAttached = cell.TryGetComponent(out _cartingRoadComponent);
            _cellEntity = cell;
            if(_cellEntity == null) return;
            SceneView.duringSceneGui += OnSceneGUIUpdate;
        }

        private void OnDisable()
        {
            if(_cellEntity == null) return;
            SceneView.duringSceneGui -= OnSceneGUIUpdate;
        }

        private void OnSceneGUIUpdate(SceneView obj)
        {
            if (!_isRoadComponentAttached) return;
            var orientation = _cellEntity.GetOrientation();

            var inDir = _cartingRoadComponent.InDirection switch
                        {
                            RoadDirection.None => Vector3.zero,
                            RoadDirection.North => Vector3.back,
                            RoadDirection.East => Vector3.left,
                            RoadDirection.South => Vector3.forward,
                            RoadDirection.West => Vector3.right,
                            _ => throw new ArgumentOutOfRangeException()
                        };

            var outDir = _cartingRoadComponent.OutDirection switch
                         {
                             RoadDirection.None => Vector3.zero,
                             RoadDirection.North => Vector3.back,
                             RoadDirection.East => Vector3.left,
                             RoadDirection.South => Vector3.forward,
                             RoadDirection.West => Vector3.right,
                             _ => throw new ArgumentOutOfRangeException()
                         };
            var dir = inDir - outDir;
            var rotation = orientation.Rotation * dir;
            if (rotation == Vector3.zero) return;
            Handles.color = Handles.zAxisColor;
            var cellSize = _cellEntity.CellSize;
            var scaledDir = Vector3.Scale(new Vector3(size, size, size), dir);

            var arrowMiddlePosition =
                orientation.Position + (cellSize - scaledDir) / 2;
            var halfSizeMagnitude = (cellSize / 3f).magnitude;
            Handles.DrawLine(arrowMiddlePosition + inDir * -1 * halfSizeMagnitude, arrowMiddlePosition);

            Handles.DrawLine(arrowMiddlePosition + scaledDir, arrowMiddlePosition + outDir * -1 *
                                                              halfSizeMagnitude + scaledDir);

            Handles.ArrowHandleCap(
                                   0,
                                   orientation.Position + (cellSize - scaledDir) / 2,
                                   Quaternion.LookRotation(rotation),
                                   scaledDir.magnitude,
                                   EventType.Repaint
                                  );
        }
    }
}
