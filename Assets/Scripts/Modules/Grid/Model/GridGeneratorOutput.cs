using SubModules.Cell.Interfaces;
using SubModules.Cell.Model;

namespace Modules.Grid.Model
{
    public readonly struct GridGeneratorOutput
    {
        public ICellEntity[,] Grid { get; }
        public RoadDirection GridExit { get; }
        public int RoadLenght { get; }

        public GridGeneratorOutput(ICellEntity[,] grid, RoadDirection gridExit, int roadLenght)
        {
            Grid = grid;
            GridExit = gridExit;
            RoadLenght = roadLenght;
        }
    }
}
