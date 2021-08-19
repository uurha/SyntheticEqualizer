using UnityEngine;

namespace Cells.Interfaces
{
    public interface IInstantiable
    {
        public IInstantiable CreateInstance(Transform parent);
    }
}