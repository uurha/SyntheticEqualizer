using System;
using Base;
using Base.BaseTypes;
using Base.Deque;
using CorePlugin.Attributes.EditorAddons;
using CorePlugin.Attributes.Headers;
using CorePlugin.Cross.Events.Interface;
using CorePlugin.Extensions;
using Extensions;
using Modules.Grid.Model;
using Modules.Grid.SubSystems.Generator;
using SubModules.Beom;
using SubModules.Cell.Interfaces;
using SubModules.Cell.Model;
using UnityEngine;

namespace Modules.Grid.SubSystems.GridInitializer
{
    [CoreManagerElement]
    public class GridInitializer : MonoBehaviour, IEventHandler
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
        private EntityRoute _previousGridExit;

        private event GridEvents.GridConfigurationChangedEvent OnGridChanged;

        public bool IsInitialized { get; private set; }

        public Conveyor<GridConfiguration> InstancedGrids { get; private set; }

        // Start is called before the first frame update
        private void Start()
        {
            Initialize();
        }

        [EditorButton("Generate Next Grid")]
        public void GenerateNextGrid()
        {
            if (!Application.isPlaying) return;
            _gridGenerator.GenerateNextGrid(OnGridGenerated);
        }

        [EditorButton("Initialize")]
        public void Initialize()
        {
            if (!Application.isPlaying) return;

            if (IsInitialized)
            {
                IsInitialized = false;

                if (InstancedGrids != null)
                    foreach (var gridConfiguration in InstancedGrids)
                        gridConfiguration.Clear();
            }
            InstancedGrids = new Conveyor<GridConfiguration>(maxGridCount, configuration => configuration.Clear());
            _initialPosition = Vector3.zero;
            InstantiateGrid(preset);
        }

        private void InstantiateGrid(BeomPreset beomPreset)
        {
            _gridGenerator = new GridGenerator(columnCount, rowCount, beomPreset.GetBeomEntities(), seedValue);
            _gridGenerator.GenerateGrid(OnGridGenerated);
        }

        private void OnGridGenerated(GridGeneratorOutput gridGeneratorOutput)
        {
            var bufferList = new ICellEntity[columnCount, rowCount];
            var bufferGrid = gridGeneratorOutput.Grid;
            var previousGrid = InstancedGrids.IsEmpty ? default : InstancedGrids.Last;
            var lineSize = Vector3.zero;

            for (var row = 0; row < rowCount; row++)
            {
                for (var column = 0; column < columnCount; column++)
                {
                    var cellEntity = (ICellEntity) bufferGrid[column, row].CreateInstance(transform);
                    var positionInGrid = new TupleInt(column, row);
                    if (previousGrid.IsInitialized) lineSize = previousGrid.LineSize(_previousGridExit, positionInGrid);
                    var bufferPosition = new Orientation(_initialPosition + lineSize, Quaternion.identity);
                    cellEntity.Initialize().SetOrientation(cellEntity.Orient(bufferPosition, positionInGrid));
                    cellEntity.Name = $"{cellEntity.Name} {column}X{row}";
                    bufferList[column, row] = cellEntity;
                }
            }
            var gridConfiguration = new GridConfiguration(bufferList);
            InstancedGrids.AddLast(gridConfiguration);
            _previousGridExit = gridGeneratorOutput.GridExit;
            var entity = gridConfiguration.ColumnsConfiguration[0].GetCells()[0];
            _initialPosition = entity.GetOrientation().Position;
            IsInitialized = true;
            OnGridChanged?.Invoke(InstancedGrids);
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
    }
}
