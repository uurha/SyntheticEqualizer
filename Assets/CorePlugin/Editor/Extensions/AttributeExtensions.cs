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
using CorePlugin.Attributes.Editor;
using CorePlugin.Attributes.Validation.Base;
using CorePlugin.Logger;
using UnityEngine;

namespace CorePlugin.Editor.Extensions
{
    internal static class AttributeExtensions
    {
        public static IEnumerable<ErrorObjectPair> ErrorObjectPairs(GameObject o)
        {
            var behaviours = o.GetComponentsInChildren<MonoBehaviour>(true);
            return ListErrors(behaviours);
        }

        public static IEnumerable<ErrorObjectPair> ErrorObjectPairs()
        {
            var behaviours = Object.FindObjectsOfType<MonoBehaviour>();
            return ListErrors(behaviours);
        }

        private static IEnumerable<ErrorObjectPair> ListErrors(IEnumerable<MonoBehaviour> behaviours)
        {
            var listErrors = new List<ErrorObjectPair>();

            foreach (var b in behaviours)
            {
                var fields = b.GetType().GetFieldsAttributes<FieldValidationAttribute>();

                foreach (var field in fields)
                {
                    foreach (var attribute in field.Value)
                    {
                        if (attribute.Validate(field.Key, b)) continue;
                        listErrors.Add(new ErrorObjectPair(attribute.ErrorMessage, b));
                        if (attribute.ShowError) ShowError(attribute.ErrorMessage, b);
                    }
                }
                var attributes = b.GetType().GetClassAttributes<ClassValidationAttribute>();

                foreach (var attribute in attributes)
                {
                    if (attribute.Validate(b)) continue;
                    listErrors.Add(new ErrorObjectPair(attribute.ErrorMessage, b));
                    if (attribute.ShowError) ShowError(attribute.ErrorMessage, b);
                }
            }
            return listErrors;
        }

        public static void ShowError(string msg, Object o)
        {
            DebugLogger.LogError(msg.Replace("\n", " "), o);
        }
    }
}
