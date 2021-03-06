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
using UnityEngine;
using UnityEngine.UI;

namespace CorePlugin.Console.ConsoleElements
{
    /// <summary>
    /// Log toggle for <see cref="CorePlugin.Console.RuntimeConsole"/>
    /// </summary>
    [RequireComponent(typeof(Toggle))]
    public class ToggleCountDisplayer : CountDisplayer
    {
        private Toggle _toggle;

        public override CountDisplayer Initialize(ConsoleIcons icons)
        {
            base.Initialize(icons);
            _toggle = GetComponent<Toggle>();
            return this;
        }

        private void SetActiveIcon(bool state)
        {
            icon.sprite = _icons.GetLogIconSprite(designatedType, state);
        }

        public override CountDisplayer SetInteractionAction(Action<LogType, bool> onInteractWithDisplayer)
        {
            _toggle.onValueChanged.AddListener(state => onInteractWithDisplayer?.Invoke(designatedType, state));
            _toggle.onValueChanged.AddListener(SetActiveIcon);
            return this;
        }
    }
}
