using UnityEngine;

namespace Grid
{
    public class CellEntity : MonoBehaviour, ICellEntity
    {
        [SerializeField] private EntityRoute inDirection;
        [SerializeField] private EntityRoute outDirection;
        [SerializeField] private Vector3 cellSize;

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