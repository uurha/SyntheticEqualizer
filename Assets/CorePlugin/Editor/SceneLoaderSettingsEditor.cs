#region license

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

using CorePlugin.SceneManagement;
using UnityEditor;

namespace CorePlugin.Editor
{
    /// <summary>
    /// Editor for <seealso cref="CorePlugin.SceneManagement.SceneLoaderSettings"/> validation
    /// </summary>
    [CustomEditor(typeof(SceneLoaderSettings))]
    public class SceneLoaderSettingsEditor : UnityEditor.Editor
    {
        private SceneLoaderSettings sceneLoaderSettings;

        private void OnEnable()
        {
            sceneLoaderSettings = (SceneLoaderSettings) target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            SceneLoaderSettingsValidator.ValidateScenesLoaderSettings(sceneLoaderSettings);
        }
    }
}