using System;
using System.Linq;
using CorePlugin.Attributes.Headers;
using Extensions;
using UnityEngine;

namespace Grid
{
    public class GridInitializer : MonoBehaviour
    {
        [PrefabHeader]
        [SerializeField] private BeomPreset preset;

        [SettingsHeader]
        [SerializeField] private int rowCount;
        [SerializeField] private int columnCount;
        [SerializeField] private int seedValue;

        private GridConfiguration _gridConfiguration;
        private bool _isInitialized;
        private GridGenerator _gridGenerator;

        public bool IsInitialized => _isInitialized;

        public GridConfiguration InstancedGrid => _gridConfiguration;


        // Start is called before the first frame update
        private void Start()
        {
            Initialize();
        }

        public void Initialize()
        {
            if (!Application.isPlaying) return;

            if (_isInitialized)
            {
                _isInitialized = false;
                _gridConfiguration.Clear();
            }
            InstantiateGrid(preset);
        }

        public void GenerateNextGrid()
        {
            _gridGenerator.GenerateNextGrid(preset.GetBeomEntities(), OnGridGenerated);
        }

        private void InstantiateGrid(BeomPreset preset)
        {
            _gridGenerator = new GridGenerator(columnCount, rowCount, seedValue);
            _gridGenerator.GenerateGrid(preset.GetBeomEntities(), OnGridGenerated);
        }

        private void OnGridGenerated(GridGeneratorOutput gridGeneratorOutput)
        {
            var bufferList = new ICellEntity[columnCount, rowCount];
            var bufferGrid = gridGeneratorOutput.Grid;

            var additionalColumn = 0;
            var additionalRow = 0;

            if (_gridConfiguration.IsInitialized)
            {
                switch (gridGeneratorOutput.LastDirection)
                {
                    case EntityRoute.North:
                        additionalRow = _gridConfiguration.ColumnLenght;
                        break;
                    case EntityRoute.East:
                        additionalColumn = _gridConfiguration.RowLenght;
                        break;
                    case EntityRoute.South:
                        additionalRow = _gridConfiguration.ColumnLenght * -1;
                        break;
                    case EntityRoute.West:
                        additionalColumn = _gridConfiguration.RowLenght * -1;
                        break;
                }
            }

            for (var row = 0; row < rowCount; row++)
            {
                for (var column = 0; column < columnCount; column++)
                {
                    var cellEntity = (ICellEntity) bufferGrid[column, row].CreateInstance(transform);

                    cellEntity.Initialize()
                              .SetOrientation(cellEntity.Orient(column + additionalColumn, row + additionalRow));
                    cellEntity.Name = $"{cellEntity.Name} {column}X{row}";
                    bufferList[column, row] = cellEntity;
                }
            }

            _gridConfiguration = new GridConfiguration(bufferList);
            _isInitialized = true;
        }
    }

    public enum EntityRoute
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
}