using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin.Attributes.Headers;
using CorePlugin.Attributes.Validation;
using Extensions;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Grid
{
    public class GridInitializer : MonoBehaviour
    {
        [PrefabHeader]
        [SerializeField] private BeomPreset preset;

        [SettingsHeader]
        [SerializeField] private int rowCount;
        [SerializeField] private int columnCount;

        private GridConfiguration _gridConfigurations;
        private bool _isInitialized;

        public bool IsInitialized => _isInitialized;

        public GridConfiguration InstancedGrid => _gridConfigurations;


        // Start is called before the first frame update
        private void Start()
        {
            Initialize();
        }

        [ContextMenu("Initialize")]
        private void Initialize()
        {
            if(!Application.isPlaying) return;

            if (_isInitialized)
            {
                _isInitialized = false;
                _gridConfigurations.Clear();
            }
            _gridConfigurations = InstantiateGrid(preset);
            _isInitialized = true;
        }

        private GridConfiguration InstantiateGrid(BeomPreset preset)
        {
            var bufferList = new ICellEntity[columnCount, rowCount];
            var generatedPath = GeneratePath(columnCount, rowCount);
            var grid = GenerateCellGrid(generatedPath ,columnCount, rowCount, preset.GetBeomEntities());

            for (var row = 0; row < rowCount; row++)
            {
                for (var column = 0; column < columnCount; column++)
                {
                    var cellEntity = grid[column, row].CreateInstance(transform);
                    cellEntity.Initialize().Orient(Orient(cellEntity, column, row));
                    cellEntity.Name = $"{cellEntity.Name} {column}X{row}";
                    bufferList[column, row] = cellEntity;
                }
            }

            return new GridConfiguration(bufferList);
        }

        private ICellEntity[,] GenerateCellGrid(Queue<EntityType> roadPath, int columns, int rows, BeomCells preset)
        {
            var cellGrid = new ICellEntity[columns, rows];
            var entityType = roadPath.Dequeue();
            var cell = preset.GetCell(entityType.Negative(), entityType);
            var randomInitialColumn = Random.Range(0, columns);
            var previousCellEntity = cellGrid[randomInitialColumn, 0] = cell;

            var currentColumn = randomInitialColumn;
            var currentRow = 0;

            for (int i = 0; i < roadPath.Count; i++)
            {
                var next = roadPath.Dequeue();
                var inDir = previousCellEntity.OutDirection.Negative();
                var nextCell = preset.GetCell(inDir, next);
                
                var prevColumn = currentColumn;
                var prevRow = currentRow;

                if (nextCell == null)
                {
                    Debug.Log("f");
                }
                
                cellGrid[currentColumn, currentRow] = nextCell;
                previousCellEntity = nextCell;
                

                switch (next)
                {
                    case EntityType.North:
                        currentRow++;
                        break;
                    case EntityType.East:
                        currentColumn++;
                        break;
                    case EntityType.South:
                        currentRow--;
                        break;
                    case EntityType.West:
                        currentColumn--;
                        break;
                }

                if (currentColumn >= columns ||
                    currentColumn < 0)
                {
                    if (next == EntityType.East ||
                        next == EntityType.West)
                    {
                        cellGrid[prevColumn, currentRow] = nextCell;
                        break;
                    }
                }

                if (currentRow >= rows ||
                    currentRow < 0)
                {
                    if (next == EntityType.North ||
                        next == EntityType.South)
                    {
                        cellGrid[currentColumn, prevRow] = nextCell;
                        break;
                    }
                }
            }

            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    cellGrid[column, row] ??= preset.GetCell(EntityType.None, EntityType.None);
                }
            }

            return cellGrid;
        }

        private Queue<EntityType> GeneratePath(int columnCount, int rowCount)
        {
            var count = columnCount * rowCount;
            var ids = new Queue<EntityType>(count);

            var random = new System.Random();
            for (int i = 0; i < count; i++)
            {
                if (i <= 3)
                {
                    ids.Enqueue(EntityType.North);
                    continue;
                }

                var randomNextStep = ids.ToArray()[i - 1].Negative();

                while (true)
                {
                    var bufferStep = RandomDirection(random);

                    if (bufferStep != randomNextStep)
                    {
                        randomNextStep = bufferStep;
                        break;
                    }
                }
                ids.Enqueue(randomNextStep);

            }

            return ids;
        }
        
        private EntityType[,] GenerateGrid(int columnCount, int rowCount)
        {
            var ids = new EntityType[columnCount , rowCount];
            
            return ids;
        }

        private Vector3 Orient(ICellEntity prefabEntity, int x, int z)
        {
            return new Vector3(prefabEntity.CellSize.x * x, 0f,
                               prefabEntity.CellSize.z * z);
        }

        private EntityType RandomDirection(System.Random random)
        {
            var values = Enum.GetValues(typeof(EntityType));
            return (EntityType)values.GetValue(random.Next((int)EntityType.None + 1, values.Length));
        }
    }

    public enum EntityType
    {
        None = 0,
        North = -1,
        East = 2,
        South = 1,
        West = -2
    }
    

    public enum Direction
    {
        In,
        Out
    }

    public struct GridConfiguration
    {
        private LineConfiguration[] _rowConfiguration;
        private LineConfiguration[] _columnsConfiguration;
        private ICellEntity[] _lineConfigurations;

        public GridConfiguration(ICellEntity[,] cellEntities)
        {
            _rowConfiguration =
                cellEntities.FillDimension(MatrixDimension.Row, entities => new LineConfiguration(entities));

            _columnsConfiguration =
                cellEntities.FillDimension(MatrixDimension.Column, entities => new LineConfiguration(entities));
            _lineConfigurations = _columnsConfiguration.Concat(_rowConfiguration).SelectMany(x => x.GetCells()).ToArray();
        }

        public LineConfiguration[] RowConfiguration => _rowConfiguration;
        public LineConfiguration[] ColumnsConfiguration => _columnsConfiguration;

        public void CalculateCellInBound(Func<Vector3, bool> predicate)
        {
            foreach (var cellEntity in _lineConfigurations)
            {
                var visible = predicate.Invoke(cellEntity.GetOrientation().Position);
                cellEntity.SetActive(visible);
            }
        }

        public void Clear()
        {
            foreach (var cellEntity in _lineConfigurations)
            {
                cellEntity.Destroy();
            }
            _rowConfiguration = new LineConfiguration[0];
            _columnsConfiguration = new LineConfiguration[0];
            _lineConfigurations = new ICellEntity[0];
        }
    }
}