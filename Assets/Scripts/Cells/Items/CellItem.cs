using System;
using UnityEngine;

namespace Cells.Items
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
