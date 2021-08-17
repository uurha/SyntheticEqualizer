using UnityEngine;

namespace Cell.Interfaces
{
    public interface IInstantiable
    {
        public IInstantiable CreateInstance(Transform parent);
    }
}