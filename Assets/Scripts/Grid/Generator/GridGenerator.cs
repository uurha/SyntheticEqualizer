﻿using System;
using System.Threading.Tasks;
using Base.BaseTypes;
using Base.Deque;
using BeomSystem;
using Cells.Interfaces;
using Cells.Model;
using Extensions;
using Grid.Model;
using Route.Generator;
using UnityEngine;
using TaskExtensions = Extensions.TaskExtensions;

namespace Grid.Generator
{
    public class GridGenerator
    {
        private readonly int _columnsCount;
        private readonly int _rowCount;
        private readonly BeomCells _preset;
        private readonly int _seed;
        private Deque<EntityRoute> _generatedPath;
        private int _lastColumn;
        private int _lastRow;
        private EntityRoute _lastEntity;
        private RouteGenerator _pathGenerator;
        
        [Serializable]
        private class LoopInOut
        {
            public ICellEntity PreviousCellEntity { get; set; }
            public TupleInt CurrentPosition { get; set; }
        }

        public GridGenerator(int columnsCount, int rowCount, BeomCells preset, int seed)
        {
            _columnsCount = columnsCount;
            _rowCount = rowCount;
            _preset = preset;
            _seed = seed;
        }

        /// <summary>
        /// Generates grid based on internal PathGenerator
        /// </summary>
        /// <param name="action"></param>
        public async void GenerateGrid(Action<GridGeneratorOutput> action)
        {
            _pathGenerator = new RouteGenerator(_columnsCount * _rowCount, _seed);
            _generatedPath = await _pathGenerator.GeneratePathAsync();
            _lastColumn = 0;
            _lastRow = 0;
            _lastEntity = EntityRoute.None;
            
            var generatedGrid = await GenerateGridAsync(_generatedPath);
            action?.Invoke(generatedGrid);
        }

        /// <summary>
        /// Generates next grid part based on internal path generated by PathGenerator
        /// </summary>
        /// <param name="action"></param>
        /// <remarks>If path lenght less then stored row count or column count generates additional path part</remarks>
        public async void GenerateNextGrid(Action<GridGeneratorOutput> action)
        {
            for (int i = 0; i < 2; i++)
            {
                _generatedPath.AddLast(_generatedPath.Last);
            }

            if (_generatedPath.Count <= _rowCount ||
                _generatedPath.Count <= _columnsCount)
            {
                _generatedPath.AddRangeFirst(await _pathGenerator.GeneratePathAsync());
            }
            
            var generatedGrid = await GenerateGridAsync(_generatedPath);
            action?.Invoke(generatedGrid);
        }

        /// <summary>
        /// Generates grid based on externally passed PathGenerator
        /// </summary>
        /// <param name="action"></param>
        /// <param name="pathGenerator"></param>
        /// <remarks>Do not stores passed path or PathGenerator</remarks>
        public async void GenerateGrid(Action<GridGeneratorOutput> action, RouteGenerator pathGenerator)
        {
            var generatedPath = await pathGenerator.GeneratePathAsync();
            _lastColumn = 0;
            _lastRow = 0;
            _lastEntity = EntityRoute.None;
            
            var generatedGrid = await GenerateGridAsync(generatedPath);
            action?.Invoke(generatedGrid);
        }

        /// <summary>
        /// Generates next grid part based on path generated by externally passed PathGenerator.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="pathGenerator"></param>
        /// <remarks>Do not stores passed path or PathGenerator</remarks>
        public async void GenerateNextGrid(Action<GridGeneratorOutput> action, RouteGenerator pathGenerator)
        {
            var generatedPath = await pathGenerator.GeneratePathAsync();
            
            var generatedGrid = await GenerateGridAsync(generatedPath);
            action?.Invoke(generatedGrid);
        }

        private Task<GridGeneratorOutput> GenerateGridAsync(Deque<EntityRoute> generatedPath)
        {
            return TaskExtensions.CreateTask(() => GenerateGrid(generatedPath));
        }

        private GridGeneratorOutput GenerateGrid(Deque<EntityRoute> generatedPath)
        {
            var cellGrid = new ICellEntity[_columnsCount, _rowCount];

            var currentPosition = StartPosition(_lastEntity);

            var entityType = EntityRoute.None;

            if (_lastEntity == EntityRoute.None)
            {
                entityType = generatedPath.Last;
                generatedPath.RemoveLast();
            }
            else
            {
                entityType = _lastEntity;
            }
            
            var cell = _preset.GetCell(entityType.Negative(), entityType);

            var stepInOut = new LoopInOut
                            {
                                CurrentPosition = currentPosition,
                                PreviousCellEntity = cell
                            };
            
            cellGrid[currentPosition.Item1, currentPosition.Item2] = cell;

            var count = 0;
            var breakCount = _columnsCount * _rowCount;

            while (count <= breakCount)
            {
                if (LoopStep(generatedPath, cellGrid, ref stepInOut)) break;
                count++;
            }

            for (var row = 0; row < _rowCount; row++)
            {
                for (var column = 0; column < _columnsCount; column++)
                {
                    cellGrid[column, row] ??= _preset.GetCell(EntityRoute.None, EntityRoute.None);
                }
            }

            _lastColumn = currentPosition.Item1;
            _lastRow = currentPosition.Item2;
            _lastEntity = stepInOut.PreviousCellEntity.OutDirection;
            Debug.Log($"Remaining Count: {generatedPath.Count}");

            return new GridGeneratorOutput
                   {
                       Grid = cellGrid,
                       GridExit = _lastEntity
                   };
        }

        private TupleInt StartPosition(EntityRoute entityRoute)
        {
            var currentPosition = new TupleInt();
            switch (entityRoute)
            {
                case EntityRoute.South:
                    currentPosition.Item1 = _lastColumn;
                    currentPosition.Item2 = _rowCount - 1;
                    break;
                case EntityRoute.North:
                    currentPosition.Item1 = _lastColumn;
                    currentPosition.Item2 = 0;
                    break;
                case EntityRoute.West:
                    currentPosition.Item1 = _columnsCount - 1;
                    currentPosition.Item2 = _lastRow;
                    break;
                case EntityRoute.East:
                    currentPosition.Item1 = 0;
                    currentPosition.Item2 = _lastRow;
                    break;
                case EntityRoute.None:
                    currentPosition.Item1 = _columnsCount / 2;
                    currentPosition.Item2 = 0;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return currentPosition;
        }
        
        private bool LoopStep(Deque<EntityRoute> generatedPath, ICellEntity[,] cellGrid, ref LoopInOut loopInOut)
        {
            var next = generatedPath.RemoveLast();
            var inDir = loopInOut.PreviousCellEntity.OutDirection.Negative();
            var nextCell = _preset.GetCell(inDir, next);
            
            var currentPosition = loopInOut.CurrentPosition;
            var prevPosition = new TupleInt(currentPosition);
            
            cellGrid[currentPosition.Item1, currentPosition.Item2] = nextCell;
            loopInOut.PreviousCellEntity = nextCell;

            switch (next)
            {
                case EntityRoute.North:
                    currentPosition.Item2++;
                    break;
                case EntityRoute.East:
                    currentPosition.Item1++;
                    break;
                case EntityRoute.South:
                    currentPosition.Item2--;
                    break;
                case EntityRoute.West:
                    currentPosition.Item1--;
                    break;
                case EntityRoute.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if ((currentPosition.Item1 >= _columnsCount || currentPosition.Item1 < 0) &&
                (next == EntityRoute.East || next == EntityRoute.West))
            {
                currentPosition.Item1 = prevPosition.Item1;
                cellGrid[prevPosition.Item1, currentPosition.Item2] = nextCell;
                return true;
            }

            if ((currentPosition.Item2 >= _rowCount || currentPosition.Item2 < 0) &&
                (next == EntityRoute.North || next == EntityRoute.South))
            {
                currentPosition.Item2 = prevPosition.Item2;
                cellGrid[currentPosition.Item1, prevPosition.Item2] = nextCell;
                return true;
            }
            return false;
        }
    }
}
