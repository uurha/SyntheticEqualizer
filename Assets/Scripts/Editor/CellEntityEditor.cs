using System;
using System.Linq;
using Cell;
using Cell.Interfaces;
using CorePlugin.Attributes.Editor;
using Grid;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(CellEntity))]
    public class CellEntityEditor : ValidationAttributeEditor
    {
        private float size = 1f;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            SceneView.duringSceneGui += OnSceneGUIUpdate;
        }
        
        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUIUpdate;
        }

        public void OnSceneGUIUpdate(SceneView obj)
        {
            if (!(target is ICellEntity targetObject)) return;

            var orientation = targetObject.GetOrientation();

            var inDir = targetObject.InDirection switch
                        {
                            EntityRoute.None => Vector3.zero,
                            EntityRoute.North => Vector3.back,
                            EntityRoute.East => Vector3.left,
                            EntityRoute.South => Vector3.forward,
                            EntityRoute.West => Vector3.right,
                            _ => throw new ArgumentOutOfRangeException()
                        };

            var outDir = targetObject.OutDirection switch
                         {
                             EntityRoute.None => Vector3.zero,
                             EntityRoute.North => Vector3.back,
                             EntityRoute.East => Vector3.left,
                             EntityRoute.South => Vector3.forward,
                             EntityRoute.West => Vector3.right,
                             _ => throw new ArgumentOutOfRangeException()
                         };
            
            var dir = inDir - outDir;
            var rotation = orientation.Rotation * dir;
            
            if(rotation == Vector3.zero) return;

            Handles.color = Handles.zAxisColor;
            var cellSize = targetObject.CellSize;
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
