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
using CorePlugin.Attributes.EditorAddons;
using CorePlugin.Core;
using CorePlugin.Core.Interface;
using CorePlugin.Editor.Extensions;
using CorePlugin.Extensions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CorePlugin.Editor.Windows
{
    public class CoreSelectorWindow : EditorWindow
    {
        private readonly string[] _strings = {"(Clone)", "Core"};
        private List<Named<Object, List<Object>>> _cores = new List<Named<Object, List<Object>>>();
        private int _mainTab;
        private int _subTab = -1;

        private UnityEditor.Editor _embeddedInspector;

        private void OnEnable()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void FindCores()
        {
            if (UnityExtensions.TryToFindObjectsOfType(out IList<ICore> cores))
                foreach (var core in cores)
                {
                    var coreObject = (MonoBehaviour) core;

                    coreObject.FindComponentWithAttribute<CoreManagerElementAttribute>(elementList =>
                                                                                       {
                                                                                           var list = elementList.ToList();
                                                                                           if (!list.Any()) return;
                                                                                           _cores.Add(coreObject, list);
                                                                                       });
                }
        }

        public static void Init()
        {
            // Get existing open window or if none, make a new one:
            var window = (CoreSelectorWindow) GetWindow(typeof(CoreSelectorWindow), true, nameof(CoreSelectorWindow).PrettyCamelCase());
            window.Show();
            window.FindCores();
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            if (_embeddedInspector != null) DestroyImmediate(_embeddedInspector);
        }

        private void OnGUI()
        {
            if (!CoreManager.ReadyForWindow)
            {
                Close();
                return;
            }
            if (_cores.Count <= 0) return;
            var bufferTab = _mainTab;

            _mainTab = GUILayout.Toolbar(_mainTab, _cores.Select(x => x.Key.PrettyObjectName(_strings)).ToArray(), new GUIStyle(GUI.skin.button)
                                             {
                                                 stretchWidth = true
                                             });
            if (_mainTab != bufferTab) _subTab = -1;

            if (_mainTab <= _cores.Count)
            {
                var key = _cores[_mainTab];
                var displayObject = key.Key;

                _subTab = GUILayout.Toolbar(_subTab, key.Value.Select(x => x.PrettyEditorObjectName(_strings)).ToArray(), new GUIStyle(GUI.skin.button)
                                                {
                                                    stretchWidth = true
                                                });
                if (_subTab != -1) displayObject = key.Value[_subTab];
                RecycleInspector(displayObject);
                _embeddedInspector.OnInspectorGUI();
            }
        }

        private void OnPlayModeStateChanged(PlayModeStateChange obj)
        {
            switch (obj)
            {
                case PlayModeStateChange.EnteredEditMode:
                    break;
                case PlayModeStateChange.ExitingEditMode:
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    _cores.Clear();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(obj), obj, null);
            }
        }

        // a helper method that destroys the current inspector instance before creating a new one
        // use this in place of "Editor.CreateEditor"
        private void RecycleInspector(Object target)
        {
            if (_embeddedInspector != null) DestroyImmediate(_embeddedInspector);
            _embeddedInspector = UnityEditor.Editor.CreateEditor(target);
        }
    }
}
