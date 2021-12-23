using System;
using System.Linq;
using System.Threading.Tasks;
using Base;
using Base.BaseTypes;
using Base.Deque;
using CorePlugin.Attributes.EditorAddons;
using CorePlugin.Attributes.Headers;
using CorePlugin.Cross.Events.Interface;
using CorePlugin.Extensions;
using Extensions;
using Modules.Carting.Interfaces;
using Modules.GlobalSettings.Presets;
using Modules.Grid.Interfaces;
using Modules.Grid.Model;
using Modules.Grid.Systems.Generator;
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
        private async Task GenerateNextGrid()
        {
            if (!Application.isPlaying) return;
            OnGridGenerated(await _gridGenerator.GenerateNextGrid());
        }

        [EditorButton("Initialize")]
        public void Initialize()
        {
            if (!Application.isPlaying) return;

            if (_isInitialized)
            {
                _isInitialized = false;

                if (_instancedGrids != null)
                    foreach (var gridConfiguration in _instancedGrids) gridConfiguration.Clear();
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

        private async void OnGridGenerated(GridGeneratorOutput gridGeneratorOutput)
        {
            var bufferGrid = new IChunkComponent[columnCount, rowCount];
            var bufferRoad = new ICartingRoad[gridGeneratorOutput.RoadPositions.Count];
            var bufferGeneratedGrid = gridGeneratorOutput.Grid;
            var previousGrid = _instancedGrids.IsEmpty ? default : _instancedGrids.Last;
            var lineSize = Vector3.zero;

            for (var index = 0; index < gridGeneratorOutput.RoadPositions.Count; index++)
            {
                var (column, row) = gridGeneratorOutput.RoadPositions[index];

                var buffer = InstantiateCell(bufferGeneratedGrid, column, row, previousGrid,
                                             ref lineSize);
                bufferGrid[column,row] = buffer;
                if (buffer.IsRoad) bufferRoad[index] =  buffer.CartingRoadComponent;
                await Task.Yield();
            }

            for (var row = 0; row < rowCount; row++)
            {
                for (var column = 0; column < columnCount; column++)
                {
                    
                    if(gridGeneratorOutput.RoadPositions.Contains(new TupleInt(column, row), new TupleInt.TupleValueComparer())) continue;
                    bufferGrid[column, row] =
                        InstantiateCell(bufferGeneratedGrid, column, row, previousGrid, ref lineSize);
                    
                    await Task.Yield();
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

        private IChunkComponent InstantiateCell(IChunkComponent[,] bufferGeneratedGrid, int column, int row,
                                               GridConfiguration previousGrid,
                                               ref Vector3 lineSize)
        {
            var cellEntity = (IChunkComponent)bufferGeneratedGrid[column, row].CreateInstance(transform);
            var positionInGrid = new TupleInt(column, row);
            if (previousGrid.IsInitialized) lineSize = previousGrid.LineSize(_previousGridExit, positionInGrid);
            var initialPosition = _initialPosition + lineSize;
            var bufferPosition = new Orientation(initialPosition, Quaternion.identity, Vector3.zero);

            return cellEntity.Initialize($"{cellEntity.Name} {column}X{row}")
                             .SetOrientation(cellEntity.Orient(bufferPosition, positionInGrid));
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
