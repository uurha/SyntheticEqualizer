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

using CorePlugin.Attributes.Validation;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CorePlugin.UISystem.UI
{
    public class ButtonWithText : MonoBehaviour
    {
        [SerializeField] [NotNull] private Button button;
        [SerializeField] [NotNull] private TMP_Text textLabel;

        public UnityEvent onClick => button.onClick;

        public Color color
        {
            get => button.image.color;
            set => button.image.color = value;
        }

        public string text
        {
            set => textLabel.text = value;
            get => textLabel.text;
        }
    }
}
