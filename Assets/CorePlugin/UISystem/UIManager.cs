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

using System.Linq;
using CorePlugin.Attributes.Headers;
using CorePlugin.Attributes.Validation;
using CorePlugin.Core;
using CorePlugin.Logger;
using CorePlugin.UISystem.UI;
using UnityEngine;
using UnityEngine.Events;

namespace CorePlugin.UISystem
{
    /// <summary>
    /// Base implementation of UIManager.
    /// </summary>
    [RequireComponent(typeof(SubPagesController))]
    public class UIManager : BaseCore
    {
        [ReferencesHeader]
        [SerializeField] [NotNull] private Transform pageButtonHolder;

        [SerializeField] [NotNull] private Transform uiPageHolder;
        private SubPagesController _subPagesController;

        public override void InitializeElements()
        {
            _subPagesController = GetComponent<SubPagesController>();

            foreach (var o in elements.Select(m => Instantiate(m, uiPageHolder)))
            {
                #if DEBUG
                DebugLogger.Log($"Create element: {o.name}");
                #endif
                if (!o.TryGetComponent(out UIPage page)) continue;
                var pageButton = Instantiate(page.PageButtonPrefab, pageButtonHolder);
                _subPagesController.AddPage(page.Initialize(), out UnityAction action);
                pageButton.onClick.AddListener(action);
                pageButton.text = page.PageName;
            }
            _subPagesController.Initialize();
        }
    }
}
