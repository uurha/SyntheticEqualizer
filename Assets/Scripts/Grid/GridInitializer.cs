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
            
            var bufferRow = FillRow(bufferList, gridSize.z, gridSize.x);
            var bufferColumn = FillColumn(bufferList, gridSize.x, gridSize.z);
            
            return new GridConfiguration(bufferRow, bufferColumn);
        }

        private LineConfiguration[] FillRow(ICellEntity[,] bufferList, int lineCount, int lineSize)
        {
            var bufferLine = new LineConfiguration[lineCount];

            for (var z = 0; z < lineCount; z++)
            {
                var lineItems = new ICellEntity[lineSize];

                for (var x = 0; x < lineSize; x++)
                {
                    lineItems[x] = bufferList[x, z];
                }
                bufferLine[z] = new LineConfiguration(lineItems);
            }
            return bufferLine;
        }
        
        private LineConfiguration[] FillColumn(ICellEntity[,] bufferList, int lineCount, int lineSize)
        {
            var bufferLine = new LineConfiguration[lineCount];

            for (var z = 0; z < lineCount; z++)
            {
                var lineItems = new ICellEntity[lineSize];

                for (var x = 0; x < lineSize; x++)
                {
                    lineItems[x] = bufferList[z, x];
                }
                bufferLine[z] = new LineConfiguration(lineItems);
            }
            return bufferLine;
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
        private IEnumerable<ICellEntity> _lineConfigurations;

        public GridConfiguration(LineConfiguration[] rowConfiguration, LineConfiguration[] columnsConfiguration)
        {
            _rowConfiguration = rowConfiguration;
            _columnsConfiguration = columnsConfiguration;
            _lineConfigurations = _columnsConfiguration.Concat(_rowConfiguration).SelectMany(x=>x.GetCells());
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