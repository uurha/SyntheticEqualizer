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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CorePlugin.Editor.Extensions;
using CorePlugin.SceneManagement;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CorePlugin.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(SceneLoaderAsset))]
    public class SceneLoaderAssetDrawer : PropertyDrawer
    {
        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);
            var target = property.serializedObject.targetObject;
            var value = fieldInfo.GetValue(target);
            DrawItem(position, value, property, target, label);
            EditorGUI.EndProperty();
        }

        private void DrawItem(Rect position, object value, SerializedProperty property, Object targetObject, GUIContent label)
        {
            SceneLoaderAsset sceneManagerAsset;
            List<SceneLoaderAsset> bufferList = null;
            var index = -1;

            switch (value)
            {
                case List<SceneLoaderAsset> list:
                {
                    var s = Regex.Match(property.propertyPath, @"\[(.*?)\]").Value.Trim('[', ']');
                    index = int.Parse(s);
                    sceneManagerAsset = list[index];
                    bufferList = list;
                    label = new GUIContent($"Element {index}");
                    break;
                }
                case SceneLoaderAsset asset:
                    sceneManagerAsset = asset;
                    break;
                default:
                    return;
            }
            SceneAsset oldScene = null;

            if (sceneManagerAsset.InstanceID != 0)
            {
                var path = AssetDatabase.GetAssetPath(sceneManagerAsset.InstanceID);
                oldScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);

                if (!string.Equals(path, sceneManagerAsset.FullPath, StringComparison.Ordinal))
                {
                    CheckSceneLoaderAsset(oldScene, targetObject, bufferList, index);
                    var t = property.serializedObject.hasModifiedProperties;
                    EditorUtility.SetDirty(targetObject);
                    return;
                }
            }
            EditorGUI.BeginChangeCheck();
            position = new Rect(position.x, position.y + 1f, position.width, EditorGUIUtility.singleLineHeight);
            var newScene = EditorGUI.ObjectField(position, label, oldScene, typeof(SceneAsset), false) as SceneAsset;
            if (!EditorGUI.EndChangeCheck()) return;
            CheckSceneLoaderAsset(newScene, targetObject, bufferList, index);
            EditorUtility.SetDirty(targetObject);
        }

        private void CheckSceneLoaderAsset(SceneAsset newScene, Object target, IList<SceneLoaderAsset> bufferList, int index)
        {
            var newPath = AssetDatabase.GetAssetPath(newScene);
            SceneLoaderAsset newManagerAsset = null;
            if (newScene is { }) newManagerAsset = new SceneLoaderAsset(newPath, newScene.GetInstanceID());

            if (bufferList == null)
            {
                fieldInfo.SetValue(target, newManagerAsset);
            }
            else
            {
                bufferList[index] = newManagerAsset;
                fieldInfo.SetValue(target, bufferList);
            }
            CheckBuildScene(newPath, newScene);
        }

        private static void CheckBuildScene(string path, Object sceneToCheck)
        {
            if (sceneToCheck == null) return;
            var buildScene = EditorBuildSettings.scenes.FirstOrDefault(x => x.path == path);

            if (buildScene == null)
                UnityEditorExtension.HelpBox($"Scene <b>{sceneToCheck.name}</b> not in build. Add scene to SceneLoaderSettings.", MessageType.Error);
            else if (!buildScene.enabled)
                UnityEditorExtension.HelpBox($"Scene <b>{sceneToCheck.name}</b> not enabled in build settings", MessageType.Error);
        }
    }
}
