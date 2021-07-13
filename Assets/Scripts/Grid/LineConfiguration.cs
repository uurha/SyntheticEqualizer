using UnityEngine;

namespace Grid
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
    }
}
