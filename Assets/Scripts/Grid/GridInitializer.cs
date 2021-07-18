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
            _gridConfigurations = InstantiateGrid(preset);
            _isInitialized = true;
        }

        private GridConfiguration InstantiateGrid(BeomPreset preset)
        {
            var bufferList = new ICellEntity[columnCount, rowCount];

            var grid = GenerateCellGrid(GenerateGrid(columnCount, rowCount), preset.GetBeomEntities());

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

        private ICellEntity[,] GenerateCellGrid(EntityType[,] typeGrid, BeomCells preset)
        {
            var rowSize = typeGrid.GetLength((int)MatrixDimension.Row);
            var columnSize = typeGrid.GetLength((int)MatrixDimension.Column);
            var cellGrid = new ICellEntity[columnSize, rowSize];

            for (var row = 0; row < rowSize; row++)
            {
                for (var column = 0; column < columnSize; column++)
                {
                    cellGrid[column, row] = preset.GetCell(typeGrid[column, row]);
                }
            }

            return cellGrid;
        }

        private EntityType[,] GenerateGrid(int columnCount, int rowCount)
        {
            var ids = new EntityType[columnCount, rowCount];

            for (var row = 0; row < rowCount; row++)
            {
                var bufferRow = new EntityType[columnCount];
                var fillRowWithMovables = false;
                for (var column = 0; column < columnCount; column++)
                {
                    if (row == 0)
                    {
                        if (column == Mathf.RoundToInt(columnCount / 2f))
                        {
                            bufferRow[column] = EntityType.Movable;
                        }
                        else
                        {
                            bufferRow[column] = EntityType.Unmovable;
                        }
                        continue;
                    }

                    if (!fillRowWithMovables)
                    {
                        var prevRow = ids.GetRow(row - 1);

                        if (prevRow.Any(x => x == EntityType.Left || x == EntityType.Right))
                        {
                            if (prevRow.Count(x => x == EntityType.Left || x == EntityType.Right) >= 1)
                            {
                                bufferRow[column] =
                                    (EntityType) Random.Range((int) EntityType.Unmovable,
                                                              (int) EntityType.AnyUnmovable);
                                continue;
                            }
                        }

                        if (prevRow[column].IsMovable())
                        {
                            bufferRow[column] =
                                (EntityType) Random.Range((int) EntityType.Movable, (int) EntityType.AnyMovable);

                            switch (bufferRow[column])
                            {
                                case EntityType.Left:
                                    column = -1;
                                    fillRowWithMovables = true;
                                    continue;
                                case EntityType.Right:
                                    fillRowWithMovables = true;
                                    continue;
                            }
                        }
                        else if (prevRow[column].IsUnMovable())
                        {
                            bufferRow[column] =
                                (EntityType) Random.Range((int) EntityType.Unmovable, (int) EntityType.AnyUnmovable);
                        }
                    }
                    else
                    {
                        switch (bufferRow[column])
                        {
                            case EntityType.Left:
                                fillRowWithMovables = false;
                                continue;
                            case EntityType.Right:
                                fillRowWithMovables = false;
                                continue;
                        }

                        if (bufferRow.Count(x => x == EntityType.Left || x == EntityType.Right) < 2)
                        {
                            var entityType =
                                (EntityType) Random.Range((int) EntityType.Movable, (int) EntityType.AnyMovable);

                            bufferRow[column] = entityType;

                            if (bufferRow.Count(x => x == entityType) % 2 == 0)
                            {
                                bufferRow[column] = EntityType.Movable;
                            }
                        }
                        else
                        {
                            bufferRow[column] = EntityType.Movable;
                        }
                    }
                }
                ids.SetRow(row, bufferRow);
            }

            return ids;
        }

        private Vector3 Orient(ICellEntity prefabEntity, int x, int z)
        {
            return new Vector3(prefabEntity.CellSize.x * x, 0f,
                               prefabEntity.CellSize.z * z);
        }
    }
        
    public enum EntityType : int
    {
        AnyUnmovable = Unmovable - 1,
        Unmovable = Any -1,
        Any = 0,
        Movable = Any + 1,
        Left = Movable + 1,
        Right = Left + 1,
        AnyMovable = Right + 1
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

        public void CalculateCellInBound(Func<Vector3, bool> predicate)
        {
            foreach (var cellEntity in _lineConfigurations)
            {
                var visible = predicate.Invoke(cellEntity.GetOrientation().Position);
                cellEntity.SetActive(visible);
            }
        }
    }
}