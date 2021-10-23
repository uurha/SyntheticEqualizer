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
using CorePlugin.Attributes.Headers;
using CorePlugin.Attributes.Validation;
using CorePlugin.Console.ConsoleElements;
using UnityEngine;

namespace CorePlugin.Console
{
    /// <summary>
    /// Initialize minimized and maximized console
    /// </summary>
    [OneAndOnly]
    public class ConsoleInitializer : MonoBehaviour
    {
        [SettingsHeader]
        [SerializeField] private bool initializeMinimized;
        [NotNull] [SerializeField] private ConsoleIcons icons;

        [ReferencesHeader]
        [NotNull] [SerializeField] private RuntimeConsole maximizedConsole;
        [NotNull] [SerializeField] private MinimizedConsole minimizedConsole;

        private void Awake()
        {
            #if DEBUG || ENABLE_RELEASE_CONSOLE
            Initialize();
            #else
            Destroy(gameObject);
            #endif
        }

        private void Initialize()
        {
            if (FindObjectsOfType<RuntimeConsole>().Length > 1)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
            minimizedConsole.Initialize(OnConsoleMaximized, icons).SetActive(initializeMinimized);

            maximizedConsole.OnLogCountUpdated +=
                minimizedConsole.CountDisplayers.Aggregate<CountDisplayer, Action<HashSet<LogType>, int>>(null,
                    (current, displayer) => current + displayer.OnLogCountChanged);
            maximizedConsole.Initialize(OnConsoleMinimized, icons).SetActive(!initializeMinimized);
        }

        private void OnConsoleMaximized()
        {
            maximizedConsole.SetActive(true);
        }

        private void OnConsoleMinimized()
        {
            minimizedConsole.SetActive(true);
        }
    }
}
