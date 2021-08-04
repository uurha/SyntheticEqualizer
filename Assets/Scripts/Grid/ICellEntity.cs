using UnityEngine;

namespace Grid
{
    public interface IInstantiable
    {
        public IInstantiable CreateInstance(Transform parent);
    }
    
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

        public void Destroy();

        public Orientation GetOrientation();
    }

    public struct CellEntityStruct
    {
        public Vector3 CellSize { get; }
        
        public EntityRoute InDirection { get; }
        
        public EntityRoute OutDirection { get; }
        
        public string Name { get; set; }

        public Orientation Orientation { get; set; }

        public CellEntityStruct(ICellEntity cellEntity)
        {
            CellSize = cellEntity.CellSize;
            InDirection = cellEntity.InDirection;
            OutDirection = cellEntity.OutDirection;
            Name = cellEntity.Name;
            Orientation = cellEntity.GetOrientation();
        }
    }
}
