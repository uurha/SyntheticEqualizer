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

namespace CorePlugin.Attributes.Validation
{
    /// <summary>
    /// Attribute that require implementation of the provided interface.
    /// </summary>
    public class RequireInterfaceAttribute : PropertyAttribute
    {
        private readonly Type _requiredType;

        /// <summary>
        /// Requiring implementation of the <see cref="T:CorePlugin.Attributes.Validation.RequireInterfaceAttribute"/> interface.
        /// </summary>
        /// <param name="type">Interface type.</param>
        public RequireInterfaceAttribute(Type type)
        {
            _requiredType = type;
        }

        // Interface type.
        public Type RequiredType => _requiredType;
    }
}
