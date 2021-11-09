using System.Collections.Generic;
using Base.BaseTypes;
using Modules.Grid.Interfaces;

namespace Modules.Grid.Model
{
    public readonly struct GridGeneratorOutput
    {
        public ICellComponent[,] Grid { get; }
        public RoadDirection GridExit { get; }
        public List<TupleInt> RoadPositions { get; }

        public GridGeneratorOutput(ICellComponent[,] grid, RoadDirection gridExit, List<TupleInt> roadPositions)
        {
            Grid = grid;
            GridExit = gridExit;
            RoadPositions = roadPositions;
        }
    }
}
