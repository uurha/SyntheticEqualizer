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
using UnityEngine;

namespace CorePlugin.SceneManagement
{
    /// <summary>
    /// Class represents SceneAsset for <seealso cref="CorePlugin.SceneManagement.SceneLoader"/> 
    /// </summary>
    [Serializable]
    public class SceneLoaderAsset
    {
        [SerializeField] private string fullPath;
        [SerializeField] private string name;
        [SerializeField] private int instanceID;

        public string FullPath => fullPath;

        public string Name => name;

        public int InstanceID => instanceID;

        public SceneLoaderAsset(string path, int instanceID)
        {
            fullPath = path;
            this.instanceID = instanceID;
            name = Path.GetFileNameWithoutExtension(path);
        }

        public bool Validate()
        {
            return !string.IsNullOrEmpty(fullPath) && !string.IsNullOrEmpty(name) && instanceID != 0;
        }
    }
}
