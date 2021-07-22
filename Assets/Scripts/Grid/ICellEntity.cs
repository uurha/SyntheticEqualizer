using System;
using UnityEngine;

namespace Grid
{
    public interface ICellEntity
    {
        public Vector3 CellSize { get; }

        public EntityType InDirection { get; }
        public EntityType OutDirection { get; }
        
        public string Name { get; set; }

        public ICellEntity Initialize();

        public void SetActive(bool state);

        public ICellEntity Orient(Vector3 position);

        public ICellEntity Orient(Quaternion rotation);

        public ICellEntity Orient(Vector3 position, Quaternion rotation);

        public ICellEntity CreateInstance(Transform parent);

        public void Destroy();

        public Orientation GetOrientation();
    }

    public struct CellEntityStruct
    {
        public Vector3 CellSize { get; }
        
        public EntityType InDirection { get; }
        
        public EntityType OutDirection { get; }
        
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
