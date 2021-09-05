using CellModule.Interfaces;
using CellModule.Model;

namespace GridModule.Model
{
    public class GridGeneratorOutput
    {
        public ICellEntity[,] Grid { get; set; }
        public EntityRoute GridExit { get; set; }
    }
}