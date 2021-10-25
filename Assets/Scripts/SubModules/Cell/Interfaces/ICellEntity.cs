using Base.BehaviourModel.Interfaces;
using SubModules.Cell.Model;
using UnityEngine;

namespace SubModules.Cell.Interfaces
{
    public interface ICellEntity : IInstantiable
    {
        public Vector3 CellSize { get; }

        public RoadDirection InDirection { get; }

        public RoadDirection OutDirection { get; }
        
        public bool IsRoad { get; }

        public ICellVisualBehaviour VisualBehaviour { get; }

        public string Name { get; set; }

        public ICellEntity Initialize();
        
        public ICellEntity Initialize(string objectName);

        public void SetActive(bool state);

        public ICellEntity SetOrientation(Vector3 position);

        public ICellEntity SetOrientation(Quaternion rotation);

        public ICellEntity SetOrientation(Vector3 position, Quaternion rotation);

        public ICellEntity SetOrientation(Orientation orientation);

        public void Destroy();

        public Orientation GetOrientation();
    }
}
