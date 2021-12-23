using System;
using CorePlugin.Attributes.EditorAddons.SelectAttributes;
using UnityEditor;

namespace CorePlugin.Editor.Drawers.SelectDrawers
{
    [CustomPropertyDrawer(typeof(SelectImplementationAttribute))]
    public class SelectImplementationDrawer : SelectDrawerBase<SelectImplementationAttribute>
    {
        private protected override void ValidateType(SerializedProperty property, int selectedTypeIndex, Type currentObjectType)
        {
            if (selectedTypeIndex < 0 ||
                selectedTypeIndex >= _reflectionType.Count ||
                currentObjectType == _reflectionType[selectedTypeIndex])
                return;
            currentObjectType = _reflectionType[selectedTypeIndex];
            property.managedReferenceValue = currentObjectType == null
                                                 ? null
                                                 : Activator.CreateInstance(currentObjectType);
        }
    }
}
