using UnityEngine;

namespace SubModules.Cell.Interfaces
{
    public interface IInstantiable
    {
        public IInstantiable CreateInstance(Transform parent);
    }
}
