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
using System.Reflection;
using CorePlugin.Attributes.Editor;
using CorePlugin.Core;
using UnityEditor;
using UnityEngine;

namespace CorePlugin.Editor
{
    /// <summary>
    /// Custom Editor CoreManager class.
    /// <seealso cref="CorePlugin.Core.CoreManager"/>
    /// </summary>
    [CustomEditor(typeof(CoreManager))]
    public class CoreManagerEditor : ValidationAttributeEditor
    {

        private readonly SymbolDefiner _symbolDefiner = new SymbolDefiner();
        private CoreManager _manager;

        protected override void OnEnable()
        {
            base.OnEnable();
            _manager = (CoreManager) target;
            _symbolDefiner.OnEnable();
        }

        private static IEnumerable<FieldInfo> GetConstants(Type type)
        {
            var fieldInfos = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            return fieldInfos.Where(fi => fi.IsLiteral && !fi.IsInitOnly).ToList();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            foreach (var value in from fieldInfo in GetConstants(typeof(CoreManager))
                                  select (string) fieldInfo.GetValue(_manager)
                                  into value
                                  where !TagHelper.IsTagsExistsInternal(value)
                                  where GUILayout.Button($"Define Tag: {value}")
                                  select value)
                TagHelper.AddTag(value);
            _symbolDefiner.ShowSymbolsButtons();
        }
    }
}
