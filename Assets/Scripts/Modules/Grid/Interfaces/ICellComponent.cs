using Base.BehaviourModel.Interfaces;
using Modules.Carting.Interfaces;
using Modules.Grid.Model;
using UnityEngine;

namespace Modules.Grid.Interfaces
{
    public interface ICellComponent : IInstantiable
    {
        public Vector3 CellSize { get; }
        
        public bool IsRoad { get; }
        
        public ICartingRoadComponent CartingRoadComponent { get; } 

        public ICellVisualBehaviour VisualBehaviourComponent { get; }

        public string Name { get; set; }

        public ICellComponent Initialize();
        
        public ICellComponent Initialize(string objectName);

        public void SetActive(bool state);

        public ICellComponent SetOrientation(Vector3 position);

        public ICellComponent SetOrientation(Quaternion rotation);

        public ICellComponent SetOrientation(Vector3 position, Quaternion rotation);

        public ICellComponent SetOrientation(Orientation orientation);

        public void Destroy();

        public Orientation GetOrientation();
    }
}
