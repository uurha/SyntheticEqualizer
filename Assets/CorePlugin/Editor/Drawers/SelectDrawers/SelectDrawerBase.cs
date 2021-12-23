﻿using System;
using System.Collections.Generic;
using System.Linq;
using CorePlugin.Attributes.EditorAddons.SelectAttributes;
using UnityEditor;
using UnityEngine;

namespace CorePlugin.Editor.Drawers.SelectDrawers
{
    public abstract class SelectDrawerBase<T> : PropertyDrawer where T : SelectAttributeBase
    {
        private bool _initializeFold = false;

        private protected List<Type> _reflectionType;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.ManagedReference) return;
            var utility = (T)attribute;
            LazyGetAllInheritedType(utility.GetFieldType());
            var popupPosition = GetPopupPosition(position);

            var typePopupNameArray =
                _reflectionType.Select(type => type == null ? "null" : $"{type.Name} : {type}").ToArray();

            var typeFullNameArray = _reflectionType
                                   .Select(type => type == null
                                                       ? ""
                                                       : $"{type.Assembly.ToString().Split(',')[0]} {type.FullName}")
                                   .ToArray();

            //Get the type of serialized object 
            var currentTypeIndex = Array.IndexOf(typeFullNameArray, property.managedReferenceFullTypename);
            var currentObjectType = _reflectionType[currentTypeIndex];
            var selectedTypeIndex = EditorGUI.Popup(popupPosition, currentTypeIndex, typePopupNameArray);

            ValidateType(property, selectedTypeIndex, currentObjectType);

            if (!_initializeFold)
            {
                property.isExpanded = currentTypeIndex != 0;
                _initializeFold = true;
            }
            EditorGUI.PropertyField(position, property, label, true);
        }

        private protected abstract void ValidateType(SerializedProperty property, int selectedTypeIndex, Type currentObjectType);

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, true);
        }

        private void LazyGetAllInheritedType(Type baseType)
        {
            if (_reflectionType != null) return;

            _reflectionType = AppDomain.CurrentDomain.GetAssemblies()
                                       .SelectMany(s => s.GetTypes())
                                       .Where(p => baseType.IsAssignableFrom(p) && (p.IsClass || p.IsValueType))
                                       .ToList();
            _reflectionType.Insert(0, null);
        }

        private Rect GetPopupPosition(Rect currentPosition)
        {
            var popupPosition = new Rect(currentPosition);
            popupPosition.width -= EditorGUIUtility.labelWidth;
            popupPosition.x += EditorGUIUtility.labelWidth;
            popupPosition.height = EditorGUIUtility.singleLineHeight;
            return popupPosition;
        }
    }
}
