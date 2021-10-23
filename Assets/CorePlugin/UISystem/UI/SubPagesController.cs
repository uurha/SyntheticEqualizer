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
using CorePlugin.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace CorePlugin.UISystem.UI
{
    /// <summary>
    /// Class designated for UI subpages.
    /// </summary>
    public class SubPagesController : MonoBehaviour
    {
        [SerializeField] private TMP_Text header;
        [SerializeField] private float delayTime;
        [SerializeField] private bool showFirstOnAwake = true;

        private readonly List<UIPage> _pages = new List<UIPage>();

        /// <summary>
        /// Adding new UIPage to this subpages controller
        /// </summary>
        /// <param name="page"></param>
        /// <param name="openPage"></param>
        public void AddPage(UIPage page, out Action openPage)
        {
            _pages.Add(page);
            openPage = () => { OpenPage(page); };
        }

        /// <summary>
        /// Adding new UIPage to this subpages controller
        /// </summary>
        /// <param name="page"></param>
        /// <param name="openPage"></param>
        public void AddPage(UIPage page, out UnityAction openPage)
        {
            _pages.Add(page);
            openPage = () => { OpenPage(page); };
        }

        /// <summary>
        /// Enables page interaction after delay
        /// </summary>
        /// <param name="page"></param>
        public void DelayedOpenPage(UIPage page)
        {
            DelayedOpenPage(page, delayTime);
        }

        /// <summary>
        /// Enables page interaction after delay
        /// </summary>
        /// <param name="page"></param>
        /// <param name="delay"></param>
        public void DelayedOpenPage(UIPage page, float delay)
        {
            HideAllTables();
            if (IsContains(page)) return;
            SetText(page);
            StartCoroutine(UIStateTools.ChangeGroupState(page.Group, true, delay));
        }

        /// <summary>
        /// Hides all pages in this subpage controller
        /// </summary>
        public void HideAllTables()
        {
            foreach (var page in _pages) page.Group.ChangeGroupState(false);
        }

        public void Initialize()
        {
            if (!showFirstOnAwake) return;
            HideAllTables();
            if (_pages.Count <= 0) return;
            if (_pages[0] == null) return;
            _pages[0].Group.ChangeGroupState(true);
            SetText(_pages[0]);
        }

        private bool IsContains(UIPage page)
        {
            var namedGroup = _pages.Contains(page);
            return namedGroup;
        }

        /// <summary>
        /// Showing canvas group sent thought parameter and disabling all others.
        /// </summary>
        /// <param name="page"></param>
        public void OpenPage(UIPage page)
        {
            HideAllTables();
            if (IsContains(page)) return;
            page.Group.ChangeGroupState(true);
            SetText(page);
        }

        /// <summary>
        /// Setting text to subpage header
        /// </summary>
        /// <param name="namedGroup"></param>
        private void SetText(UIPage namedGroup)
        {
            if (header != null) header.text = namedGroup.PageName;
        }
    }
}
