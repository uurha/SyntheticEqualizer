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

        // Interface type.
        public Type RequiredType => _requiredType;

        /// <summary>
        /// Requiring implementation of the <see cref="T:CorePlugin.Attributes.Validation.RequireInterfaceAttribute"/> interface.
        /// </summary>
        /// <param name="type">Interface type.</param>
        public RequireInterfaceAttribute(Type type)
        {
            _requiredType = type;
        }
    }
}
