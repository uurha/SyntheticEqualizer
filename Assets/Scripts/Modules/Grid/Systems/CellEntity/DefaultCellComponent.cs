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

        public Orientation GetOrientation(bool isLocal = false)
        {
            return new Orientation(transform, isLocal);
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

        public ICellComponent SetOrientation(Orientation orientation)
        {
            var thisTransform = transform;
            if (orientation.IsLocal)
            {
                thisTransform.localPosition = orientation.Position;
                thisTransform.localRotation = orientation.Rotation;
            }
            else
            {
                thisTransform.position = orientation.Position;
                thisTransform.rotation = orientation.Rotation;
            }
            return this;
        }
    }
}
