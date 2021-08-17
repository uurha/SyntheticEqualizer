using Cell.Interfaces;

namespace Grid.Model
{
    public class GridGeneratorOutput
    {
        public ICellEntity[,] Grid { get; set; }
        public EntityRoute GridExit { get; set; }
    }
}