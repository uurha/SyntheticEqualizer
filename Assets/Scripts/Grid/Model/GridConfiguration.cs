using System;
using Cells.Interfaces;
using Extensions;
using UnityEngine;

namespace Grid.Model
{
    public struct GridConfiguration
    {
        private LineConfiguration[] _rowConfiguration;
        private LineConfiguration[] _columnsConfiguration;
        private int _columnLenght;
        private int _rowLenght;
        private bool _isInitialized;

        public LineConfiguration[] RowConfiguration => _rowConfiguration;
        public LineConfiguration[] ColumnsConfiguration => _columnsConfiguration;

        public int ColumnLenght => _columnLenght;

        public int RowLenght => _rowLenght;

        public bool IsInitialized => _isInitialized;

        public GridConfiguration(ICellEntity[,] cellEntities)
        {
            _rowConfiguration = 
                cellEntities.FillDimension(MatrixDimension.Row, entities => new LineConfiguration(entities));

            _columnsConfiguration =
                cellEntities.FillDimension(MatrixDimension.Column, entities => new LineConfiguration(entities));
            
            _columnLenght = cellEntities.GetLength((int) MatrixDimension.Column);
            _rowLenght = cellEntities.GetLength((int) MatrixDimension.Row);
            _isInitialized = true;
        }

        public void CalculateCellInBound(Func<Vector3, bool> predicate)
        {
            for (var col = 0; col < _columnLenght; col++)
            {
                for (var row = 0; row < _rowLenght; row++)
                {
                    var gridConfiguration = _rowConfiguration[row].GetCells()[col];
                    var visible = predicate.Invoke(gridConfiguration.GetOrientation().Position);
                    gridConfiguration.SetActive(visible);
                }
            }
        }

        public Vector3 GetLineSize(MatrixDimension dimension, int index)
        {
            if (_rowConfiguration == null || _columnsConfiguration == null)
            {
                return Vector3.zero;
            }
            
            var lineSize = dimension switch
                           {
                               MatrixDimension.Row => _rowConfiguration[index].GetCells().Sum(x => x.CellSize),
                               MatrixDimension.Column => _columnsConfiguration[index].GetCells().Sum(x => x.CellSize),
                               _ => throw new ArgumentOutOfRangeException(nameof(dimension), dimension, null)
                           };
            return lineSize;
        }

        public void Clear()
        {
            for (var row = 0; row < _rowLenght; row++)
            {
                _rowConfiguration[row].DestroyAll();
            }

            _rowConfiguration = new LineConfiguration[0];
            _columnsConfiguration = new LineConfiguration[0];
            _columnLenght = 0;
            _rowLenght = 0;
            _isInitialized = false;
        }
    }
}
