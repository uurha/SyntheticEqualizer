using System;
using System.Linq;
using Base;
using Base.BaseTypes;
using Base.Deque;
using CorePlugin.Attributes.EditorAddons;
using CorePlugin.Attributes.Headers;
using CorePlugin.Cross.Events.Interface;
using CorePlugin.Extensions;
using Extensions;
using Modules.Carting.Interfaces;
using Modules.Grid.Interfaces;
using Modules.Grid.Model;
using Modules.Grid.Systems.Generator;
using SubModules.Beom;
using UnityEngine;

namespace Modules.Grid.Systems.Initializer
{
    [CoreManagerElement]
    public class GridInitializer : MonoBehaviour, IEventHandler, IEventSubscriber
    {
        [SettingsHeader]
        [SerializeField] private int columnCount;

        [SerializeField] private int rowCount;
        [Min(1)] [SerializeField] private int maxGridCount;
        [SerializeField] private int seedValue;

        [PrefabHeader]
        [SerializeField] private BeomPreset preset;

        private GridGenerator _gridGenerator;
        private Vector3 _initialPosition = Vector3.zero;
        private RoadDirection _previousGridExit;
        private bool _isInitialized;
        private Conveyor<GridConfiguration> _instancedGrids;

        private event GridEvents.GridConfigurationChangedEvent OnGridChanged;

        // Start is called before the first frame update
        private void Start()
        {
            Initialize();
        }

        [EditorButton("Generate Next Grid")]
        private void GenerateNextGrid()
        {
            if (!Application.isPlaying) return;
            _gridGenerator.GenerateNextGrid(OnGridGenerated);
        }

        [EditorButton("Initialize")]
        public void Initialize()
        {
            if (!Application.isPlaying) return;

            if (_isInitialized)
            {
                _isInitialized = false;

                if (_instancedGrids != null)
                {
                    foreach (var gridConfiguration in _instancedGrids) gridConfiguration.Clear();
                }
            }
            _instancedGrids ??= new Conveyor<GridConfiguration>(maxGridCount, configuration => configuration.Clear());
            _initialPosition = Vector3.zero;
            OnGridChanged?.Invoke(_instancedGrids, _isInitialized);
            InstantiateGrid(preset);
        }

        private void InstantiateGrid(BeomPreset beomPreset)
        {
            _gridGenerator = new GridGenerator(columnCount, rowCount, beomPreset.GetBeomEntities(), seedValue);
            _gridGenerator.GenerateGrid(OnGridGenerated);
        }

        private void OnGridGenerated(GridGeneratorOutput gridGeneratorOutput)
        {
            var bufferGrid = new ICellComponent[columnCount, rowCount];
            var bufferRoad = new ICartingRoadComponent[gridGeneratorOutput.RoadPositions.Count];
            var bufferGeneratedGrid = gridGeneratorOutput.Grid;
            var previousGrid = _instancedGrids.IsEmpty ? default : _instancedGrids.Last;
            var lineSize = Vector3.zero;

            for (var index = 0; index < gridGeneratorOutput.RoadPositions.Count; index++)
            {
                var (column, row) = gridGeneratorOutput.RoadPositions[index];

                var buffer = InstantiateCell(bufferGeneratedGrid, column, row, previousGrid,
                                             ref lineSize);
                bufferGrid[column,row] = buffer;
                if (buffer.IsRoad) bufferRoad[index] = buffer.CartingRoadComponent;
            }

            for (var row = 0; row < rowCount; row++)
            {
                for (var column = 0; column < columnCount; column++)
                {
                    
                    if(gridGeneratorOutput.RoadPositions.Contains(new TupleInt(column, row), new TupleInt.TupleValueComparer())) continue;
                    bufferGrid[column, row] =
                        InstantiateCell(bufferGeneratedGrid, column, row, previousGrid, ref lineSize);
                }
            }
            
            var gridConfiguration = new GridConfiguration(bufferGrid, bufferRoad);
            _instancedGrids.AddLast(gridConfiguration);
            _previousGridExit = gridGeneratorOutput.GridExit;
            var entity = gridConfiguration.ColumnsConfiguration[0].GetCells()[0];
            _initialPosition = entity.GetOrientation().Position;
            _isInitialized = true;
            OnGridChanged?.Invoke(_instancedGrids, _isInitialized);
        }

        private ICellComponent InstantiateCell(ICellComponent[,] bufferGeneratedGrid, int column, int row,
                                               GridConfiguration previousGrid,
                                               ref Vector3 lineSize)
        {
            var cellEntity = (ICellComponent)bufferGeneratedGrid[column, row].CreateInstance(transform);
            var positionInGrid = new TupleInt(column, row);
            if (previousGrid.IsInitialized) lineSize = previousGrid.LineSize(_previousGridExit, positionInGrid);
            var bufferPosition = new Orientation(_initialPosition + lineSize, Quaternion.identity);

            cellEntity.Initialize($"{cellEntity.Name} {column}X{row}")
                      .SetOrientation(cellEntity.Orient(bufferPosition, positionInGrid));
            return cellEntity;
        }

        public void InvokeEvents()
        {
        }

        public void Subscribe(params Delegate[] subscribers)
        {
            EventExtensions.Subscribe(ref OnGridChanged, subscribers);
        }

        public void Unsubscribe(params Delegate[] unsubscribers)
        {
            EventExtensions.Unsubscribe(ref OnGridChanged, unsubscribers);
        }

        public Delegate[] GetSubscribers()
        {
            return new Delegate[]
                   {
                       (GridEvents.RequestNextGrid)GenerateNextGrid
                   };
        }
    }
}
