﻿#region license

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

using System.Collections.Generic;
using UnityEditor;

namespace CorePlugin.Editor.Extensions
{
    internal class EditorSceneComparer : IEqualityComparer<EditorBuildSettingsScene>
    {
        public bool Equals(EditorBuildSettingsScene s1, EditorBuildSettingsScene s2)
        {
            return s2 == null && s1 == null || s1 != null && s2 != null && s1.path == s2.path;
        }

        public int GetHashCode(EditorBuildSettingsScene scene)
        {
            var hCode = scene.path;
            return hCode.GetHashCode();
        }
    }
}
