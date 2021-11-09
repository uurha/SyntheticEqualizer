using System;
using Modules.Grid.Interfaces;

namespace Modules.Grid.Model
{
    public struct LineConfiguration
    {
        private ICellComponent[] _cells;

        public int Lenght => _cells.Length;

        public LineConfiguration(ICellComponent[] cells)
        {
            _cells = cells;
        }

        public ICellComponent[] GetCells()
        {
            return _cells;
        }

        public void DestroyAll()
        {
            foreach (var cell in _cells) cell.Destroy();
            _cells = Array.Empty<ICellComponent>();
        }
    }
}
