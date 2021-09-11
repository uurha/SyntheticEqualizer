using System;
using System.Collections.Generic;
using System.Linq;
using Base;
using Base.BaseTypes;
using Base.Deque;
using CorePlugin.Attributes.Headers;
using CorePlugin.Cross.Events.Interface;
using Extensions;
using Modules.Grid.Model;
using Modules.Grid.SubSystems.Generator;
using SubModules.Beom;
using SubModules.Cell.Interfaces;
using SubModules.Cell.Model;
using UnityEngine;

namespace Modules.Grid.SubSystems
{
    public class GridInitializer : MonoBehaviour, IEventHandler
    {
        [SerializeField] private int columnCount;

        [SettingsHeader]
        [Min(1)] [SerializeField] private int maxGridCount;

        [PrefabHeader]
        [SerializeField] private BeomPreset preset;

        [SerializeField] private int rowCount;
        [SerializeField] private int seedValue;
        private GridGenerator _gridGenerator;
        private Vector3 _initialPosition = Vector3.zero;
        private EntityRoute _previousGridExit;

        private event CrossEventsType.OnGridUpdatedEvent onGridChanged;

        public bool IsInitialized { get; private set; }

        public Conveyor<GridConfiguration> InstancedGrids { get; private set; }

        // Start is called before the first frame update
        private void Start()
        {
            Initialize();
        }

        public void GenerateNextGrid()
        {
            _gridGenerator.GenerateNextGrid(OnGridGenerated);
        }

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

        public void InvokeEvents()
        {
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
            onGridChanged?.Invoke(InstancedGrids);
        }

        public void Subscribe(IEnumerable<Delegate> subscribers)
        {
            foreach (var gridChanged in subscribers.OfType<CrossEventsType.OnGridUpdatedEvent>())
                onGridChanged += gridChanged;
        }

        public void Unsubscribe(IEnumerable<Delegate> unsubscribers)
        {
            foreach (var gridChanged in unsubscribers.OfType<CrossEventsType.OnGridUpdatedEvent>())
                onGridChanged -= gridChanged;
        }
    }
}
