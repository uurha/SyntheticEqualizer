using System;
using System.Collections.Generic;
using System.Linq;
using CorePlugin.Attributes.Headers;
using CorePlugin.Attributes.Validation;
using Extensions;
using UnityEngine;

namespace Grid
{
    public class GridInitializer : MonoBehaviour
    {
        [SerializeField] private Matrix matrix3X3;
        [PrefabHeader]
        [HasComponent(typeof(ICellEntity))] [PrefabRequired] [SerializeField] private GameObject prefab;

        [SettingsHeader]
        [SerializeField] private Vector3Int gridSize;

        private GridConfiguration _gridConfigurations;
        private bool _isInitialized;

        public bool IsInitialized => _isInitialized;

        public GridConfiguration InstancedGrid => _gridConfigurations;

        public Vector3Int GridSize => gridSize;

        // Start is called before the first frame update
        private void Start()
        {
            _gridConfigurations = InstantiateGrid(prefab);
            _isInitialized = true;
        }

        private GridConfiguration InstantiateGrid(GameObject prefab)
        {
            var prefabEntity = prefab.GetComponent<ICellEntity>();

            var bufferList = new ICellEntity[gridSize.x, gridSize.z];

            for (var z = 0; z < gridSize.z; z++)
            {
                for (var x = 0; x < gridSize.x; x++)
                {
                    var entity = Instantiate(prefab, transform).GetComponent<ICellEntity>()
                                                               .Initialize()
                                                               .Orient(Orient(prefabEntity, x, z));
                    entity.Name = $"{entity.Name} {x}X{z}";
                    bufferList[x, z] = entity;
                }
            }
            
            return new GridConfiguration(bufferList);
        }


        private Vector3 Orient(ICellEntity prefabEntity, int x, int z)
        {
            return new Vector3(prefabEntity.CellSize.x * x, 0f,
                               prefabEntity.CellSize.z * z);
        }
    }

    public struct GridConfiguration
    {
        private readonly LineConfiguration[] _rowConfiguration;
        private readonly LineConfiguration[] _columnsConfiguration;
        private readonly IEnumerable<ICellEntity> _lineConfigurations;

        public GridConfiguration(ICellEntity[,] cellEntities)
        {
            _rowConfiguration =
                cellEntities.FillDimension(MatrixDimension.Row, entities => new LineConfiguration(entities));

            _columnsConfiguration =
                cellEntities.FillDimension(MatrixDimension.Column, entities => new LineConfiguration(entities));
            _lineConfigurations = _columnsConfiguration.Concat(_rowConfiguration).SelectMany(x => x.GetCells());
        }

        public LineConfiguration[] RowConfiguration => _rowConfiguration;
        public LineConfiguration[] ColumnsConfiguration => _columnsConfiguration;

        public void SetActive(Func<Vector3, bool> predicate)
        {
            foreach (var cellEntity in _lineConfigurations)
            {
                var visible = predicate.Invoke(cellEntity.GetOrientation().Position);
                cellEntity.SetActive(visible);
            }
        }
    }
}