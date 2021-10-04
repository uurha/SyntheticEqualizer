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
using System.Reflection;
using CorePlugin.Attributes.Validation.Base;
using CorePlugin.Editor.Extensions;
using CorePlugin.Extensions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CorePlugin.Attributes.Editor
{
    /// <summary>
    /// Custom editor for Unity.Object class.
    /// If you want to create your own custom editor inherit from this class.
    /// <seealso cref="ValidationAttribute"/>
    /// <seealso cref="FieldValidationAttribute"/>
    /// <seealso cref="ClassValidationAttribute"/>
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Object), true)]
    public class ValidationAttributeEditor : UnityEditor.Editor
    {
        private IEnumerable<Attribute> _classAttributes = new ClassValidationAttribute[0];
        private IEnumerable<FieldInfo> _fields = new FieldInfo[0];

        private bool _shouldShowErrors = true;

        protected virtual void OnEnable()
        {
            _fields = AttributeValidator.GetAllFields(target.GetType());
            _classAttributes = AttributeValidator.GetAllAttributes(target.GetType());
        }

        private SerializedProperty GetSerializedProperty(FieldInfo field)
        {
            // Do not display properties marked with HideInInspector attribute
            var hideAtts = field.GetCustomAttributes(typeof(HideInInspector), true);
            return hideAtts.Length > 0 ? null : serializedObject.FindProperty(field.Name);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            foreach (var field in _fields) ValidateField(field);
            foreach (var attribute in _classAttributes) ValidateClass(attribute);
            _shouldShowErrors = serializedObject.ApplyModifiedProperties();
        }

        private void ValidateClass(Attribute attribute)
        {
            ValidateClassAttribute(attribute as ClassValidationAttribute);
        }

        private void ValidateClassAttribute(ClassValidationAttribute attribute)
        {
            if (attribute.Validate(target)) return;
            UnityEditorExtension.HelpBox(attribute.ErrorMessage, MessageType.Error);
            if (attribute.ShowError && _shouldShowErrors) AttributeValidator.ShowError(attribute.ErrorMessage, target);
        }

        private void ValidateField(FieldInfo field)
        {
            var prop = GetSerializedProperty(field);
            if (prop == null) return;
            var atts = field.GetCustomAttributes(typeof(FieldValidationAttribute), true);
            foreach (var att in atts) ValidateFieldAttribute(att as FieldValidationAttribute, field);
        }

        private void ValidateFieldAttribute(FieldValidationAttribute attribute, FieldInfo field)
        {
            if (attribute.Validate(field, target)) return;
            UnityEditorExtension.HelpBox(attribute.ErrorMessage, MessageType.Error);
            if (attribute.ShowError && _shouldShowErrors) AttributeValidator.ShowError(attribute.ErrorMessage, target);
        }
    }
}
