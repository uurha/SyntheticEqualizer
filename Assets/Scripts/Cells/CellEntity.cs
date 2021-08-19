using System;
using System.Linq;
using CellItems;
using Cells.Behaviours;
using Cells.Behaviours.Default;
using Cells.Behaviours.Interfaces;
using Cells.Interfaces;
using CorePlugin.Attributes.Headers;
using Grid;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

namespace Cells
{
    public class CellEntity : MonoBehaviour, ICellEntity, IRunBehaviour
    {
        [ReferencesHeader]
        [SerializeField] private CellItem[] leftItems;
        
        [SerializeField] private CellItem[] rightItems;
        
        [SettingsHeader]
        [SerializeField] private EntityRoute inDirection;
        [SerializeField] private EntityRoute outDirection;
        [SerializeField] private Vector3 cellSize;
        
        private IBehaviour _itemBehaviour;
        private TransformAccessArray _leftArray;
        private TransformAccessArray _rightArray;
        
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
            _itemBehaviour = new DefaultMoveBehaviour();
            _leftArray = new TransformAccessArray(leftItems.Select(x => x.transform).ToArray());
            _rightArray = new TransformAccessArray(rightItems.Select(x => x.transform).ToArray());
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

        public void RunBehaviour(NativeArray<BehaviourData> data)
        {
            _itemBehaviour.SetData(data);
            
            JobHandle handleLeft;
            JobHandle handleRight;

            switch (_itemBehaviour)
            {
                case DefaultMoveBehaviour behaviour:
                    handleLeft = behaviour.Schedule(_leftArray);
                    handleRight = behaviour.Schedule(_rightArray);
                    break;
                case DefaultRotateBehaviour behaviour:
                    handleLeft = behaviour.Schedule(_leftArray);
                    handleRight = behaviour.Schedule(_rightArray);
                    break;
                case DefaultScaleBehaviour behaviour:
                    handleLeft = behaviour.Schedule(_leftArray);
                    handleRight = behaviour.Schedule(_rightArray);
                    break;
                default:
                    throw new InvalidOperationException(nameof(_itemBehaviour), new Exception($"Unknown type assigned to {nameof(_itemBehaviour)}"));
            }
            
            handleLeft.Complete();
            handleRight.Complete();
            data.Dispose();
        }
    }
}