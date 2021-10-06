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
using CorePlugin.Attributes.Validation.Base;

namespace CorePlugin.Attributes.EditorAddons
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class EditorButtonAttribute : EditorMethodAttribute
    {
        private readonly string _displayName;
        private readonly object[] _invokeParams;

        public EditorButtonAttribute(string displayName, params object[] invokeParams)
        {
            _displayName = displayName;
            _invokeParams = invokeParams;
        }

        public EditorButtonAttribute(params object[] invokeParams)
        {
            _displayName = string.Empty;
            _invokeParams = invokeParams;
        }

        public object[] InvokeParams => _invokeParams;

        private bool IsValidName()
        {
            return !string.IsNullOrWhiteSpace(_displayName) && !string.IsNullOrWhiteSpace(_displayName);
        }

        public string GetButtonName(string methodName)
        {
            return IsValidName() ? _displayName : methodName;
        }
    }
}
