using Base.BehaviourModel.Interfaces;
using CorePlugin.Attributes.Headers;
using CorePlugin.Logger;
using SubModules.Cell.Interfaces;
using SubModules.Cell.Model;
using UnityEngine;

namespace SubModules.Cell
{
    public class CellEntity : MonoBehaviour, ICellEntity
    {
        [SerializeField] private Vector3 cellSize;

        [SettingsHeader]
        [SerializeField] private EntityRoute inDirection;

        [SerializeField] private EntityRoute outDirection;
        private ICellVisualBehaviour _behaviour;

        public Vector3 CellSize => cellSize;

        public EntityRoute InDirection => inDirection;

        public EntityRoute OutDirection => outDirection;

        public ICellVisualBehaviour VisualBehaviour => _behaviour;

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

        public ICellEntity Initialize()
        {
            if (!TryGetComponent(out _behaviour))
                DebugLogger.LogWarning($"No Visual behaviour attached to {transform.name}", this);
            return this;
        }

        public void SetActive(bool state)
        {
            if (gameObject.activeSelf != state) gameObject.SetActive(state);
        }

        public ICellEntity SetOrientation(Vector3 position)
        {
            transform.position = position;
            return this;
        }

        public ICellEntity SetOrientation(Quaternion rotation)
        {
            transform.rotation = rotation;
            return this;
        }

        public ICellEntity SetOrientation(Vector3 position, Quaternion rotation)
        {
            SetOrientation(position);
            SetOrientation(rotation);
            return this;
        }

        public ICellEntity SetOrientation(Orientation orientation)
        {
            SetOrientation(orientation.Position, orientation.Rotation);
            return this;
        }
    }
}
