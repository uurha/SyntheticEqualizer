﻿#region license

// Copyright 2021 Arcueid Elizabeth D'athemon
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

using System.Linq;
using CorePlugin.SceneManagement;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace CorePlugin.Editor
{
    /// <summary>
    /// Validator class for <seealso cref="CorePlugin.SceneManagement.SceneLoaderSettings"/>
    /// </summary>
    public static class SceneLoaderSettingsValidator
    {
        [DidReloadScripts]
        private static void ValidateOnDidReload()
        {
            if (Validate(out var settings)) return;

            Debug.LogError(ReturnErrorText(settings));
        }

        public static string ReturnErrorText(SceneLoaderSettings settings)
        {
            return (settings, settings.Scenes) switch
            {
                (null, _) => $"{nameof(SceneLoaderSettings)} missing",
                (_, null) => $"Scenes is empty in {nameof(SceneLoaderSettings)}",
                (_, { } subList) when subList.Count <= 0 => $"Scenes is empty in {nameof(SceneLoaderSettings)}",
                _ => $"Detect difference between {nameof(SceneLoaderSettings)} and scenes in Build Settings. Updating..."
            };
        }

        public static bool Validate(out SceneLoaderSettings settings)
        {
            settings = Resources.Load<SceneLoaderSettings>(nameof(SceneLoaderSettings));
            return settings != null && ValidateScenesLoaderSettings(settings);
        }

        public static bool ValidateScenesLoaderSettings(SceneLoaderSettings settings)
        {
            var scenes = settings.Scenes;

            if (scenes == null)
            {
                return false;
            }

            for (var i = scenes.Count - 1; i >= 0; i--)
            {
                if (scenes[i] == null) continue;
                if (!scenes[i].Validate()) continue;
                if (scenes.Count(x => x != null && x.Validate() && x.InstanceID == scenes[i].InstanceID) <= 1) continue;
                scenes[i] = null;
            }

            if (settings.IntermediateScene != null && settings.IntermediateScene.Validate())
                scenes.Remove(settings.IntermediateScene);

            settings.ResetSceneList(scenes);

            var sceneLoaderAssets = settings.Scenes.Where(x => x != null && x.Validate()).ToList();

            if (settings.IntermediateScene != null && settings.IntermediateScene.Validate())
                sceneLoaderAssets.Add(settings.IntermediateScene);

            var newScenes = sceneLoaderAssets.Select(loaderAsset => new EditorBuildSettingsScene(loaderAsset.FullPath, true)).ToArray();

            var editorBuildSettingsScenes = EditorBuildSettings.scenes;

            var valid = editorBuildSettingsScenes.SequenceEqual(newScenes, new EditorSceneComparer());

            if(!valid)
                EditorBuildSettings.scenes = newScenes;
            return valid;
        }
    }
}