using UnityEngine;

namespace CellModule.Interfaces
{
    public interface IInstantiable
    {
        public IInstantiable CreateInstance(Transform parent);
    }
}