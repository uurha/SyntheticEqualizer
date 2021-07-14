using System;
using UnityEngine;

namespace Grid
{
    public interface ICellEntity
    {
        public Vector3 CellSize { get; }
        
        public Matrix NearCellIDs { get; }
        public int CellID { get; }
        
        public string Name { get; set; }

        public ICellEntity Initialize();

        public void SetActive(bool state);

        public ICellEntity Orient(Vector3 position);

        public ICellEntity Orient(Quaternion rotation);

        public ICellEntity Orient(Vector3 position, Quaternion rotation);

        public Orientation GetOrientation();
    }
}
