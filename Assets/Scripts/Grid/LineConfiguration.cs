using System;
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
    
    [Serializable]
    public struct Matrix
    {
        [SerializeField] private Vector3Int[] lines;

        public int[,] GetMatrix()
        {
            var buffer = new int[3, 3];

            for (var x = 0; x < 3; x++)
            {
                for (var y = 0; y < 3; y++)
                {
                    buffer[x, y] = lines[x][y];
                }
            }
            return buffer;
        }
    }
}
