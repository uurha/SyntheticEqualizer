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
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CorePlugin.SceneManagement
{
    /// <summary>
    /// Extensions for <seealso cref="CorePlugin.SceneManagement.SceneLoader"/>
    /// </summary>
    public static class SceneLoaderExtensions
    {
        /// <summary>
        /// Unloads current Scene
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="mode"></param>
        /// <param name="onSceneReadyToSwitch"></param>
        /// <param name="onProgressChanged"></param>
        /// <returns></returns>
        public static IEnumerator SceneUnloadOperation(this Scene scene, UnloadSceneOptions mode, Action<AsyncOperation> onSceneReadyToSwitch,
                                                       Action<float> onProgressChanged = null)
        {
            var sceneOperation = SceneManager.UnloadSceneAsync(scene, mode);
            sceneOperation.allowSceneActivation = false;
            yield return new WaitUntil(() => Until(onProgressChanged, sceneOperation));
            onSceneReadyToSwitch?.Invoke(sceneOperation);
        }

        /// <summary>
        /// Awaits Until sceneOperation ready to switch
        /// </summary>
        /// <param name="onProgressChanged"></param>
        /// <param name="sceneOperation"></param>
        /// <returns></returns>
        public static bool Until(Action<float> onProgressChanged, AsyncOperation sceneOperation)
        {
            onProgressChanged?.Invoke(sceneOperation.progress);
            return sceneOperation.progress >= 0.9f;
        }

        /// <summary>
        /// Loads SceneLoaderAsset
        /// </summary>
        /// <param name="sceneAsset"></param>
        /// <param name="mode"></param>
        /// <param name="onSceneReadyToSwitch"></param>
        /// <param name="onProgressChanged"></param>
        /// <returns></returns>
        public static IEnumerator SceneLoadOperation(this SceneLoaderAsset sceneAsset, LoadSceneMode mode, Action<AsyncOperation> onSceneReadyToSwitch,
                                                     Action<float> onProgressChanged = null)
        {
            var sceneOperation = SceneManager.LoadSceneAsync(sceneAsset.Name, mode);
            sceneOperation.allowSceneActivation = false;
            yield return new WaitUntil(() => Until(onProgressChanged, sceneOperation));
            onSceneReadyToSwitch?.Invoke(sceneOperation);
        }

        /// <summary>
        /// Unloads current Scene
        /// </summary>
        /// <param name="name"></param>
        /// <param name="mode"></param>
        /// <param name="onSceneReadyToSwitch"></param>
        /// <param name="onProgressChanged"></param>
        /// <returns></returns>
        public static IEnumerator SceneUnloadOperation(this string name, UnloadSceneOptions mode, Action<AsyncOperation> onSceneReadyToSwitch,
                                                       Action<float> onProgressChanged = null)
        {
            var sceneOperation = SceneManager.UnloadSceneAsync(name, mode);
            sceneOperation.allowSceneActivation = false;
            yield return new WaitUntil(() => Until(onProgressChanged, sceneOperation));
            onSceneReadyToSwitch?.Invoke(sceneOperation);
        }
    }
}
