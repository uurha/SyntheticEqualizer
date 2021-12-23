using System;
using System.Diagnostics;
using CorePlugin.Extensions;

namespace CorePlugin.Attributes.EditorAddons.SelectAttributes
{
    [Conditional(EditorDefinition.UnityEditor)]
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class SelectTypeAttribute : SelectAttributeBase
    {
        public SelectTypeAttribute(Type type) : base(type)
        {
        }
    }
}
