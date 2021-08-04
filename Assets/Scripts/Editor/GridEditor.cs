using System;
using CorePlugin.Attributes.Editor;
using Extensions;
using Grid;
using UnityEditor;
using UnityEngine;

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

        //EditorGUILayout.Space();

        // foreach (var attachBehaviour in (AttachBehaviour[]) Enum.GetValues(typeof(AttachBehaviour)))
        // {
        //     var values = (MatrixDimension[])Enum.GetValues(typeof(MatrixDimension));
        //     EditorGUILayout.BeginHorizontal();
        //     foreach (var dimension in  values)
        //     {
        //         if (GUILayout.Button($"{attachBehaviour.ToString()} {dimension.ToString()}"))
        //         {
        //             if (obj is { }) obj.Attach(attachBehaviour, dimension);
        //         }
        //     }
        //     EditorGUILayout.EndHorizontal();
        // }
    }
}
