using System;
using Modules.Grid.Interfaces;

namespace Modules.Grid.Model
{
    public struct LineConfiguration
    {
        private IChunkComponent[] _cells;

        public int Lenght => _cells.Length;

        public LineConfiguration(IChunkComponent[] cells)
        {
            _cells = cells;
        }

        public IChunkComponent[] GetCells()
        {
            return _cells;
        }

        public void DestroyAll()
        {
            foreach (var cell in _cells) cell.Destroy();
            _cells = Array.Empty<IChunkComponent>();
        }
    }
}
