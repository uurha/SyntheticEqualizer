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
using System.IO;
using CorePlugin.Core;
using CorePlugin.Cross.SceneData;
using CorePlugin.Editor.Windows;
using CorePlugin.SceneManagement;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CorePlugin.Editor.MenuItemHelpers
{
    public static class CoreMenuItems
    {
        [MenuItem("Core/Create Core Manager")]
        private static void CreateCoreManager()
        {
            CreatePrefab<CoreManager>();
        }

        [MenuItem("Core/Show Selector Window")]
        private static void ShowSelectorWindow()
        {
            CoreSelectorWindow.Init();
        }

        [MenuItem("Core/Cross Scene Data Handler")]
        private static void CreateCrossSceneDataHandler()
        {
            CreatePrefab<SceneDataHandler>();
        }

        [MenuItem("Core/Create Scene Settings", false, 10)]
        private static void CreateNewSceneLoaderSettings()
        {
            var settings = Resources.Load<SceneLoaderSettings>(nameof(SceneLoaderSettings));
            if (settings != null) return;
            var pattern = Path.Combine(nameof(CorePlugin), nameof(Resources));
            var dataPath = Application.dataPath;
            var paths = Directory.GetDirectories(dataPath, pattern, SearchOption.TopDirectoryOnly);
            var pathToResources = Path.Combine(dataPath, nameof(CorePlugin), nameof(Resources));
            if (paths.Length < 1) Directory.CreateDirectory(pathToResources);
            var newSettings = ScriptableObject.CreateInstance<SceneLoaderSettings>();
            var relativePath = Path.Combine(GetRelativePath(dataPath, pathToResources), $"{nameof(SceneLoaderSettings)}.asset");
            AssetDatabase.CreateAsset(newSettings, relativePath);
        }

        public static string GetRelativePath(string relativeTo, string path)
        {
            var uri = new Uri(relativeTo);

            var rel = Uri.UnescapeDataString(uri.MakeRelativeUri(new Uri(path)).ToString())
                         .Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            if (rel.Contains(Path.DirectorySeparatorChar.ToString()) == false) rel = $".{Path.DirectorySeparatorChar}{rel}";
            return rel;
        }

        [MenuItem("Core/Highlight Scene Settings", false, 9)]
        private static void Highlight()
        {
            var settings = Resources.Load<SceneLoaderSettings>(nameof(SceneLoaderSettings));
            Selection.SetActiveObjectWithContext(settings, settings);
        }

        private static string PrefabPath<T>()
        {
            return Path.Combine("Prefabs", typeof(T).Name);
        }

        private static void CreatePrefab<T>() where T : MonoBehaviour
        {
            var prefabPath = PrefabPath<T>();
            var componentOrGameObject = Resources.Load<T>(prefabPath);

            if (componentOrGameObject != null)
            {
                var objects = Object.FindObjectsOfType(typeof(T));

                if (objects.Length > 0)
                {
                    foreach (var o in objects) ShowError($"Should be only one {typeof(T).Name} in scene", o);
                    return;
                }
                if (!(PrefabUtility.InstantiatePrefab(componentOrGameObject) is T prefab)) return;
                prefab.name = componentOrGameObject.name;
                prefab.transform.SetAsLastSibling();
            }
            else
            {
                var c = Path.Combine("..", "Core", nameof(Resources), prefabPath);
                var message = $"Probably you move or rename {typeof(T).Name} prefab from initial path ({c}).";
                ShowError(message);
            }
        }

        private static void ShowError(string error, Object context = null)
        {
            EditorApplication.Beep();
            Debug.LogError(error, context);
        }
    }
}
