using Modules.Grid.Interfaces;

namespace Modules.Grid.Model
{
    public readonly struct GridGeneratorOutput
    {
        public ICellComponent[,] Grid { get; }
        public RoadDirection GridExit { get; }
        public int RoadLenght { get; }

        public GridGeneratorOutput(ICellComponent[,] grid, RoadDirection gridExit, int roadLenght)
        {
            Grid = grid;
            GridExit = gridExit;
            RoadLenght = roadLenght;
        }
    }
}
