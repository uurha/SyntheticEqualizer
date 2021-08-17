using System.Collections.Generic;
using System.Linq;
using Cell.CellItem.Interfaces;
using Cell.Interfaces;
using CorePlugin.Attributes.Headers;
using CorePlugin.Attributes.Validation;
using Grid;
using UnityEngine;

namespace Cell
{
    public class CellEntity : MonoBehaviour, ICellEntity
    {
        [ReferencesHeader]
        [HasComponent(typeof(ICellItem))] 
        [SerializeField] private GameObject[] leftItems;
        
        [HasComponent(typeof(ICellItem))] 
        [SerializeField] private GameObject[] rightItems;
        
        [SettingsHeader]
        [SerializeField] private EntityRoute inDirection;
        [SerializeField] private EntityRoute outDirection;
        [SerializeField] private Vector3 cellSize;
        
        private IEnumerable<ICellEntity> _leftCellItems;
        private IEnumerable<ICellEntity> _rightCellItems;

        public Vector3 CellSize => cellSize;

        public string Name
        {
            get => gameObject.name;
            set => gameObject.name = value;
        }

        public EntityRoute InDirection => inDirection;

        public EntityRoute OutDirection => outDirection;

        public ICellEntity Initialize()
        {
            _leftCellItems = leftItems.Select(x => x.GetComponent<ICellEntity>());
            _rightCellItems = rightItems.Select(x => x.GetComponent<ICellEntity>());
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

        public IInstantiable CreateInstance(Transform parent)
        {
            return Instantiate(this, parent);
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }

        public Orientation GetOrientation()
        {
            return new Orientation(transform);
        }
    }
}