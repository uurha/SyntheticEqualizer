using System;
using System.Collections.Generic;
using System.Linq;
using Base;
using Base.BaseTypes;
using Base.Deque;
using BeomModule;
using CellModule;
using CellModule.Interfaces;
using CellModule.Model;
using CorePlugin.Attributes.Headers;
using CorePlugin.Cross.Events.Interface;
using Extensions;
using GridModule.Generator;
using GridModule.Model;
using UnityEngine;

namespace GridModule
{
    public class GridInitializer : MonoBehaviour, IEventHandler
    {
        [PrefabHeader]
        [SerializeField] private BeomPreset preset;

        [SettingsHeader]
        [Min(1)][SerializeField] private int maxGridCount;
        [SerializeField] private int rowCount;
        [SerializeField] private int columnCount;
        [SerializeField] private int seedValue;

        private Conveyor<GridConfiguration> _gridConfigurations;
        private bool _isInitialized;
        private GridGenerator _gridGenerator;
        private Vector3 _initialPosition = Vector3.zero;
        private EntityRoute _previousGridExit;

        private event CrossEventsType.OnGridUpdatedEvent onGridChanged; 

        public bool IsInitialized => _isInitialized;

        public Conveyor<GridConfiguration> InstancedGrids => _gridConfigurations;

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

                if (_gridConfigurations != null)
                    foreach (var gridConfiguration in _gridConfigurations)
                    {
                        gridConfiguration.Clear();
                    }
            }
            _gridConfigurations = new Conveyor<GridConfiguration>(maxGridCount, (configuration => configuration.Clear()));
            _initialPosition = Vector3.zero;
            InstantiateGrid(preset);
        }

        public void GenerateNextGrid()
        {
            _gridGenerator.GenerateNextGrid(OnGridGenerated);
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
            var previousGrid = _gridConfigurations.IsEmpty ? default : _gridConfigurations.Last;

            var lineSize = Vector3.zero;

            for (var row = 0; row < rowCount; row++)
            {
                for (var column = 0; column < columnCount; column++)
                {
                    var cellEntity = (ICellEntity) bufferGrid[column, row].CreateInstance(transform);

                    var positionInGrid = new TupleInt(column, row);
                    if (previousGrid.IsInitialized)
                    {
                        lineSize = previousGrid.LineSize(_previousGridExit, positionInGrid);
                    }

                    var bufferPosition = new Orientation(_initialPosition + lineSize, Quaternion.identity);
                    
                    cellEntity.Initialize().SetOrientation(cellEntity.Orient(bufferPosition, positionInGrid));
                    cellEntity.Name = $"{cellEntity.Name} {column}X{row}";
                    bufferList[column, row] = cellEntity;
                }
            }
            var gridConfiguration = new GridConfiguration(bufferList);
            _gridConfigurations.AddLast(gridConfiguration);
            _previousGridExit = gridGeneratorOutput.GridExit;
            var entity = gridConfiguration.ColumnsConfiguration[0].GetCells()[0];

            _initialPosition = entity.GetOrientation().Position;

            _isInitialized = true;
            onGridChanged?.Invoke(_gridConfigurations);
        }

        public void InvokeEvents()
        {
            
        }

        public void Subscribe(IEnumerable<Delegate> subscribers)
        {
            foreach (var gridChanged in subscribers.OfType<CrossEventsType.OnGridUpdatedEvent>())
            {
                onGridChanged += gridChanged;
            }
        }

        public void Unsubscribe(IEnumerable<Delegate> unsubscribers)
        {
            foreach (var gridChanged in unsubscribers.OfType<CrossEventsType.OnGridUpdatedEvent>())
            {
                onGridChanged -= gridChanged;
            }
        }
    }
}