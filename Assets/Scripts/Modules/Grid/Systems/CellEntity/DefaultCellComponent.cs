using Base.BehaviourModel.Interfaces;
using CorePlugin.Logger;
using Modules.Carting.Interfaces;
using Modules.Grid.Interfaces;
using Modules.Grid.Model;
using UnityEngine;

namespace Modules.Grid.Systems.CellEntity
{
    public class DefaultCellComponent : MonoBehaviour, ICellComponent
    {
        [SerializeField] private Vector3 cellSize;

        private ICellVisualBehaviour _behaviour;
        private ICartingRoadComponent _cartingRoadComponent;
        private bool _isRoad;

        public Vector3 CellSize => cellSize;

        public bool IsRoad => _isRoad;

        public ICartingRoadComponent CartingRoadComponent => _cartingRoadComponent;

        public ICellVisualBehaviour VisualBehaviourComponent => _behaviour;

        public string Name
        {
            get => gameObject.name;
            set => gameObject.name = value;
        }

        public IInstantiable CreateInstance(Transform parent)
        {
            return Instantiate(this, Vector3.zero, Quaternion.identity, parent);
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }

        public Orientation GetOrientation()
        {
            return new Orientation(transform);
        }

        public ICellComponent Initialize()
        {
            if (!TryGetComponent(out _behaviour))
                DebugLogger.LogWarning($"No Visual behaviour attached to {transform.name}", this);
            _isRoad = TryGetComponent(out _cartingRoadComponent);
            return this;
        }

        public ICellComponent Initialize(string objectName)
        {
            Name = objectName;
            return Initialize();
        }

        public void SetActive(bool state)
        {
            if (gameObject.activeSelf != state) gameObject.SetActive(state);
        }

        public ICellComponent SetOrientation(Vector3 position)
        {
            transform.position = position;
            return this;
        }

        public ICellComponent SetOrientation(Quaternion rotation)
        {
            transform.rotation = rotation;
            return this;
        }

        public ICellComponent SetOrientation(Vector3 position, Quaternion rotation)
        {
            SetOrientation(position);
            SetOrientation(rotation);
            return this;
        }

        public ICellComponent SetOrientation(Orientation orientation)
        {
            SetOrientation(orientation.Position, orientation.Rotation);
            return this;
        }
    }
}
