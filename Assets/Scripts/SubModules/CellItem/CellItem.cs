using SubModules.Cell;
using UnityEngine;

namespace SubModules.CellItem
{
    public class CellItem : MonoBehaviour
    {
        private Orientation _initialOrientation;

        public Orientation InitialOrientation => _initialOrientation;

        public CellItem Initialize()
        {
            _initialOrientation = new Orientation(transform, true);
            return this;
        }
    }
}
