using System.Collections.Generic;
using Base.BaseTypes;
using Modules.Grid.Interfaces;

namespace Modules.Grid.Model
{
    public readonly struct GridGeneratorOutput
    {
        public IChunkComponent[,] Grid { get; }
        public RoadDirection GridExit { get; }
        public List<TupleInt> RoadPositions { get; }

        public GridGeneratorOutput(IChunkComponent[,] grid, RoadDirection gridExit, List<TupleInt> roadPositions)
        {
            Grid = grid;
            GridExit = gridExit;
            RoadPositions = roadPositions;
        }
    }
}
