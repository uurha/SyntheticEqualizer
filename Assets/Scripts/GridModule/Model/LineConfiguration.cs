using CellModule.Interfaces;

namespace GridModule.Model
{
    public struct LineConfiguration
    {
        private ICellEntity[] _cells;

        public int Lenght => _cells.Length;
        
        public LineConfiguration(ICellEntity[] cells)
        {
            _cells = cells;
        }
        
        public ICellEntity[] GetCells()
        {
            return _cells;
        }

        public void DestroyAll()
        {
            foreach (var cell in _cells)
            {
                cell.Destroy();
            }
            _cells = new ICellEntity[0];
        }
    }
}
