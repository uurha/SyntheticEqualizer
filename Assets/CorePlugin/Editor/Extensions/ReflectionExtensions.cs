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
using System.Reflection;
using System.Text.RegularExpressions;

namespace CorePlugin.Editor.Extensions
{
    public static class ReflectionExtensions
    {
        #if UNITY_EDITOR
        private const BindingFlags Flags = BindingFlags.Public | BindingFlags.NonPublic |
                                           BindingFlags.Static | BindingFlags.Instance |
                                           BindingFlags.DeclaredOnly;

        public static IEnumerable<KeyValuePair<FieldInfo, IEnumerable<T>>> GetFieldsAttributes<T>(this Type t)
            where T : Attribute
        {
            return t == null
                       ? Enumerable.Empty<KeyValuePair<FieldInfo, IEnumerable<T>>>()
                       : t.GetFields(Flags)
                          .ToDictionary(key => key, value => value.GetCustomAttributes(true).OfType<T>())
                          .Concat(GetFieldsAttributes<T>(t.BaseType));
        }

        public static string GetPrettyMemberName(this MemberInfo input)
        {
            return Regex.Replace(input.Name, "((?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z]))", " $1").Trim();
        }

        public static IEnumerable<KeyValuePair<MethodInfo, IEnumerable<T>>> GetMethodsAttributes<T>(this Type t)
            where T : Attribute
        {
            return t == null
                       ? Enumerable.Empty<KeyValuePair<MethodInfo, IEnumerable<T>>>()
                       : t.GetMethods(Flags).Where(x => x.GetCustomAttributes<T>().Any())
                          .ToDictionary(key => key, value => value.GetCustomAttributes<T>(true))
                          .Concat(GetMethodsAttributes<T>(t.BaseType));
        }

        public static IEnumerable<T> GetClassAttributes<T>(this Type t) where T : Attribute
        {
            return t == null
                       ? Enumerable.Empty<T>()
                       : t.GetCustomAttributes().OfType<T>().Concat(GetClassAttributes<T>(t.BaseType));
        }
        #endif
    }
}
