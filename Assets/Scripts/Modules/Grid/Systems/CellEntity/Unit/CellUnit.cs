using Modules.Grid.Model;
using UnityEngine;

namespace Modules.Grid.Systems.CellEntity.Unit
{
    public class CellUnit : MonoBehaviour
    {
        private Orientation _initialOrientation;

        public Orientation InitialOrientation => _initialOrientation;

        public CellUnit Initialize()
        {
            _initialOrientation = new Orientation(transform, true);
            return this;
        }
    }
}
