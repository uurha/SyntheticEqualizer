using System;
using System.Diagnostics;
using CorePlugin.Extensions;
using UnityEngine;

namespace CorePlugin.Attributes.EditorAddons.SelectAttributes
{
    [Conditional(EditorDefinition.UnityEditor)]
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public abstract class SelectAttributeBase : PropertyAttribute
    {
        Type m_type;

        public SelectAttributeBase(Type type)
        {
            m_type = type;
        }

        public Type GetFieldType()
        {
            return m_type;
        }
    }
}
