using Grid;
using UnityEngine;

namespace Cells.Interfaces
{
    public interface ICellEntity : IInstantiable
    {
        public Vector3 CellSize { get; }

        public EntityRoute InDirection { get; }
        
        public EntityRoute OutDirection { get; }
        
        public string Name { get; set; }

        public ICellEntity Initialize();

        public void SetActive(bool state);

        public ICellEntity SetOrientation(Vector3 position);

        public ICellEntity SetOrientation(Quaternion rotation);

        public ICellEntity SetOrientation(Vector3 position, Quaternion rotation);
        
        public ICellEntity SetOrientation(Orientation orientation);

        public void Destroy();

        public Orientation GetOrientation();
    }
}
