using UnityEngine;

namespace Modules.Grid.Interfaces
{
    public interface IInstantiable
    {
        public IInstantiable CreateInstance(Transform parent);
    }
}
