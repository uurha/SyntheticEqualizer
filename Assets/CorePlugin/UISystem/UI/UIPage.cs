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

using System.Collections.Generic;
using System.Linq;
using CorePlugin.Attributes.Headers;
using CorePlugin.Attributes.Validation;
using CorePlugin.Logger;
using UnityEngine;

namespace CorePlugin.UISystem.UI
{
    /// <summary>
    /// UI page implementation
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class UIPage : MonoBehaviour
    {
        [SerializeField] private string pageName;

        [PrefabHeader]
        [SerializeField] [PrefabRequired] private ButtonWithText pageButtonPrefab;

        [SerializeField] [PrefabRequired] private protected List<GameObject> elements;

        private CanvasGroup _canvasGroup;

        public ButtonWithText PageButtonPrefab => pageButtonPrefab;

        public string PageName => pageName;

        public CanvasGroup Group => _canvasGroup;

        /// <summary>
        /// Initializes elements for this UIPage
        /// </summary>
        /// <returns></returns>
        public UIPage Initialize()
        {
            _canvasGroup = GetComponent<CanvasGroup>();

            foreach (var o in elements.Select(m => Instantiate(m, transform)))
            {
                #if DEBUG
                DebugLogger.Log($"Create element: {o.name}");
                #endif
            }
            return this;
        }
    }
}
