using SubModules.Cell.Interfaces;
using SubModules.Cell.Model;

namespace Modules.Grid.Model
{
    public class GridGeneratorOutput
    {
        public ICellEntity[,] Grid { get; set; }
        public EntityRoute GridExit { get; set; }
    }
}
