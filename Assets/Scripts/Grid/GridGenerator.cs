using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Base.Deque;
using Extensions;
using TaskExtensions = Extensions.TaskExtensions;

namespace Grid
{
    public class GridGenerator
    {
        private readonly int _columnsCount;
        private readonly int _rowCount;
        private readonly int _seed;
        private Deque<EntityRoute> _generatedPath;
        private int _lastColumn;
        private int _lastRow;
        private EntityRoute _lastEntity;
        
        
        public class NextData
        {
            public int Lenght { get; set; }
            public int CurrentPosition { get; set; }
        }

        public GridGenerator(int columnsCount, int rowCount, int seed)
        {
            _columnsCount = columnsCount;
            _rowCount = rowCount;
            _seed = seed;
        }

        public async void GenerateGrid(BeomCells preset, Action<GridGeneratorOutput> action)
        {
            var path = new PathGenerator(_columnsCount * _rowCount, _seed);
            _generatedPath = await path.GeneratePathAsync();
            _lastColumn = _columnsCount / 2;
            _lastRow = 0;
            _lastEntity = _generatedPath.Last;
            
            var generatedGrid = await GenerateGridAsync(preset);
            action?.Invoke(generatedGrid);
        }

        public async void GenerateNextGrid(BeomCells preset, Action<GridGeneratorOutput> action)
        {
            for (int i = 0; i < 4; i++)
            {
                _generatedPath.AddLast(_generatedPath.Last);
            }
            var generatedGrid = await GenerateGridAsync(preset);
            action?.Invoke(generatedGrid);
        }

        private Task<GridGeneratorOutput> GenerateGridAsync(BeomCells preset)
        {
            return TaskExtensions.CreateTask(()=>GenerateGrid(preset));
        }

        private GridGeneratorOutput GenerateGrid(BeomCells preset)
        {
            var cellGrid = new ICellEntity[_columnsCount, _rowCount];
            var entityType = _lastEntity;

            var cell = preset.GetCell(entityType.Negative(), entityType);

            var previousCellEntity = cellGrid[_lastColumn, 0] = cell;

            var currentColumn = _lastColumn;
            var currentRow = _lastRow;

            for (var i = 0; i < _generatedPath.Count; i++)
            {
                var next = _generatedPath.Last;
                var inDir = previousCellEntity.OutDirection.Negative();
                var nextCell = preset.GetCell(inDir, next);

                var prevColumn = currentColumn;
                var prevRow = currentRow;

                cellGrid[currentColumn, currentRow] = nextCell;
                previousCellEntity = nextCell;

                switch (next)
                {
                    case EntityRoute.North:
                        currentRow++;
                        break;
                    case EntityRoute.East:
                        currentColumn++;
                        break;
                    case EntityRoute.South:
                        currentRow--;
                        break;
                    case EntityRoute.West:
                        currentColumn--;
                        break;
                }

                if ((currentColumn >= _columnsCount || currentColumn < 0) &&
                    (next == EntityRoute.East || next == EntityRoute.West))
                {
                    currentColumn = prevColumn;
                    cellGrid[prevColumn, currentRow] = nextCell;
                    _generatedPath.RemoveLast();
                    break;
                }

                if ((currentRow >= _rowCount || currentRow < 0) &&
                    (next == EntityRoute.North || next == EntityRoute.South))
                {
                    currentRow = prevRow;
                    cellGrid[currentColumn, prevRow] = nextCell;
                    _generatedPath.RemoveLast();
                    break;
                }

                _generatedPath.RemoveLast();
            }

            for (var row = 0; row < _rowCount; row++)
            {
                for (var column = 0; column < _columnsCount; column++)
                {
                    cellGrid[column, row] ??= preset.GetCell(EntityRoute.None, EntityRoute.None);
                }
            }

            _lastColumn = currentColumn;
            _lastRow = currentRow;
            _lastEntity = previousCellEntity.OutDirection;

            return new GridGeneratorOutput()
                   {
                       Grid = cellGrid,
                       LastDirection = _lastEntity
                   };
        }
    }

    public class GridGeneratorOutput
    {
        public ICellEntity[,] Grid { get; set; }
        public EntityRoute LastDirection { get; set; }
    }
}
