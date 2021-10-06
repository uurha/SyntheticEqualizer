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

using System.Collections.Generic;
using System.Linq;
using CorePlugin.Extensions;
using UnityEditor;
using UnityEngine;

namespace CorePlugin.Editor
{
    /// <summary>
    /// Class for "Scripting Define Symbols" defining from CoreManager Inspector.
    /// <seealso cref="CorePlugin.Core.CoreManager"/>
    /// </summary>
    public class SymbolDefiner
    {
        private readonly Dictionary<string, bool> _symbols = new Dictionary<string, bool>
                                                             {
                                                                 {EditorDefinition.EnableReleaseLogs, false},
                                                                 {EditorDefinition.EnableReleaseConsole, false}
                                                             };

        /// <summary>
        /// Shows buttons in Inspector.
        /// </summary>
        public void ShowSymbolsButtons()
        {
            var bufferSymbols = new Dictionary<string, bool>(_symbols);

            foreach (var symbol in from symbol in bufferSymbols
                                   let text = symbol.Value ? $"Define {symbol.Key}" : $"Undefine {symbol.Key}"
                                   where GUILayout.Button(text)
                                   select symbol)
            {
                _symbols[symbol.Key] = !symbol.Value;
                SetScriptingDefine(symbol);
            }
        }

        public void DefineSymbol(string key)
        {
            if (!_symbols.TryGetValue(key, out var value)) return;
            _symbols[key] = !value;
            SetScriptingDefine(_symbols.FirstOrDefault(x => x.Key.Equals(key)));
        }

        private void SetScriptingDefine(KeyValuePair<string, bool> pair)
        {
            var allDefines = AllDefines();
            allDefines.RemoveAll(string.IsNullOrWhiteSpace);

            if (pair.Value)
            {
                if (!allDefines.Contains(pair.Key)) allDefines.Add(pair.Key);
            }
            else
            {
                allDefines.RemoveAll(x => x == pair.Key);
            }

            PlayerSettings.SetScriptingDefineSymbolsForGroup(
                                                             EditorUserBuildSettings.selectedBuildTargetGroup,
                                                             string.Join(";", allDefines.ToArray()));
            AssetDatabase.Refresh();
        }

        private List<string> AllDefines()
        {
            var definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            var allDefines = definesString.Split(';').ToList();
            return allDefines;
        }

        /// <summary>
        /// Check for whether the are already defined symbols OnEnable
        /// </summary>
        public void OnEnable()
        {
            var list = AllDefines();
            var buffer = new Dictionary<string, bool>(_symbols);
            foreach (var item in buffer.Keys) _symbols[item] = !list.Contains(item);
        }
    }
}
