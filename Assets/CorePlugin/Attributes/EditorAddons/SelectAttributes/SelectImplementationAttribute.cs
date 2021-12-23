using System;
using System.Diagnostics;
using CorePlugin.Extensions;

namespace CorePlugin.Attributes.EditorAddons.SelectAttributes
{
    [Conditional(EditorDefinition.UnityEditor)]
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class SelectImplementationAttribute : SelectAttributeBase
    {
        public SelectImplementationAttribute(Type type) : base(type)
        {
        }
    }
}
