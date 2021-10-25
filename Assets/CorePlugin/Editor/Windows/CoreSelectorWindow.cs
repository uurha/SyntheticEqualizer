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
using System.Collections.Generic;
using System.Linq;
using CorePlugin.Attributes.EditorAddons;
using CorePlugin.Core;
using CorePlugin.Editor.Extensions;
using CorePlugin.Extensions;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CorePlugin.Editor.Windows
{
    public class CoreSelectorWindow : EditorWindow
    {
        private List<NamedGroup> _cores = new List<NamedGroup>();
        private int _mainTab;
        private int _subTab = -1;

        private UnityEditor.Editor _embeddedInspector;
        private static readonly string[] StringsToRemove = {"(Clone)", "Core"};

        private void OnEnable()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void DrawSelectionButton(Object displayObject)
        {
            if (GUILayout.Button($"Show <b><i>{displayObject.PrettyEditorObjectName(StringsToRemove)}</i></b> Object",
                                 new GUIStyle(GUI.skin.button) {richText = true}))
                Selection.SetActiveObjectWithContext(displayObject, displayObject);
        }

        private static IEnumerable<NamedGroup> ElementGathering(Type type, Object instance)
        {
            var field = type.GetFieldWithAttribute<CoreManagerElementsFieldAttribute>(instance);
            var empty = Enumerable.Empty<NamedGroup>();
            if (field == null) return empty;

            switch (field)
            {
                case IEnumerable enumerable:
                {
                    var list = enumerable.Cast<GameObject>().ToList();
                    if (!list.Any()) return empty;
                    var elements = list.SelectMany(x => x.GetComponentsWithAttribute<CoreManagerElementAttribute>());
                    if (EditorApplication.isPlaying) elements = elements.Select(x => FindObjectOfType(x.GetType()));

                    empty = elements.Aggregate(empty,
                                               (current, element) =>
                                                   current.Append(new NamedGroup(element,
                                                                                 element.GetCoreElementsRecursively())));
                    break;
                }
                case Object obj:
                {
                    break;
                }
            }
            return empty;
        }

        public static void Init()
        {
            // Get existing open window or if none, make a new one:
            var window = GetWindow<CoreSelectorWindow>(true, nameof(CoreSelectorWindow).PrettyCamelCase());
            window.Show();
            window.UpdateCores();
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            if (_embeddedInspector != null) DestroyImmediate(_embeddedInspector);
        }

        private void OnGUI()
        {
            if (_cores.Count <= 0) return;
            var bufferTab = _mainTab;
            var columnCount = Mathf.RoundToInt(position.width / 200);

            _mainTab = UnityEditorExtension.SelectionGrid(_mainTab, _cores.Select(x => x.NamedObject.Name).ToArray(),
                                                          columnCount,
                                                          new GUIStyle(GUI.skin.button) {stretchWidth = true});
            if (_mainTab != bufferTab) _subTab = -1;
            if (_mainTab > _cores.Count) return;
            var key = _cores[_mainTab];
            var displayObject = key.Object;
            EditorGUILayout.Separator();

            if (key.NamedObjects.Count > 0)
                EditorGUILayout.LabelField("Elements",
                                           new GUIStyle(GUI.skin.label)
                                           {
                                               stretchWidth = true, alignment = TextAnchor.MiddleCenter,
                                               fontStyle = FontStyle.BoldAndItalic
                                           });

            _subTab = UnityEditorExtension.SelectionGrid(_subTab, key.NamedObjects.Select(x => x.Name).ToArray(),
                                                         columnCount,
                                                         new GUIStyle(GUI.skin.button) {stretchWidth = true});
            if (_subTab != -1) displayObject = key.Value[_subTab].Object;
            EditorGUILayout.Separator();
            DrawSelectionButton(displayObject);
            RecycleInspector(displayObject);
            _embeddedInspector.OnInspectorGUI();
        }

        private void OnPlayModeStateChanged(PlayModeStateChange obj)
        {
            switch (obj)
            {
                case PlayModeStateChange.EnteredEditMode:
                    _cores.Clear();
                    UpdateCores();
                    break;
                case PlayModeStateChange.ExitingEditMode:
                    _cores.Clear();
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    _cores.Clear();
                    UpdateCores();
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    _cores.Clear();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(obj), obj, null);
            }
        }

        [DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            if (!HasOpenInstances<CoreSelectorWindow>()) return;
            var window = GetWindow<CoreSelectorWindow>();
            window.UpdateCores();
        }

        private void RecycleInspector(Object target)
        {
            if (_embeddedInspector != null) DestroyImmediate(_embeddedInspector);
            _embeddedInspector = UnityEditor.Editor.CreateEditor(target);
        }

        private void UpdateCores()
        {
            if (UnityExtensions.TryToFindObjectOfType<CoreManager>(out var coreManager))
                _cores = ElementGathering(typeof(CoreManager), coreManager).ToList();
        }

        private class NamedObject : Named<Object, string>
        {

            public NamedObject(Object obj) : base(obj, obj.PrettyEditorObjectName(StringsToRemove))
            {
            }

            public string Name => value;
            public Object Object => key;
        }

        private class NamedGroup : Named<NamedObject, List<NamedObject>>
        {

            public NamedGroup(NamedObject namedObject, List<NamedObject> list) : base(namedObject, list)
            {
            }

            public NamedGroup(Object namedObject, IEnumerable<Object> list) : base(new NamedObject(namedObject),
                                                                                   list.Select(item => new NamedObject(item)).ToList())
            {
            }

            public Object Object => key.Object;
            public NamedObject NamedObject => key;
            public List<NamedObject> NamedObjects => value;
        }
    }
}
