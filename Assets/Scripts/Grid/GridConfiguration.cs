using System;
using System.Linq;
using Base.Deque;
using Extensions;
using UnityEngine;

namespace Grid
{
    public enum AttachBehaviour
    {
        Append,
        Prepend
    }
    
    public struct GridConfiguration
    {
        private Deque<LineConfiguration> _rowConfiguration;
        private Deque<LineConfiguration> _columnsConfiguration;
        private ICellEntity[,] _gridConfigurations;
        private int _columnLenght;
        private int _rowLenght;
        private bool isInitialized;

        public GridConfiguration(ICellEntity[,] cellEntities)
        {
            _rowConfiguration = 
                cellEntities.FillDimension(MatrixDimension.Row, entities => new LineConfiguration(entities));

            _columnsConfiguration =
                cellEntities.FillDimension(MatrixDimension.Column, entities => new LineConfiguration(entities));
            
            _gridConfigurations = cellEntities;
            _columnLenght = _gridConfigurations.GetLength((int) MatrixDimension.Column);
            _rowLenght = _gridConfigurations.GetLength((int) MatrixDimension.Row);
            isInitialized = true;
        }

        public Deque<LineConfiguration> RowConfiguration => _rowConfiguration;
        public Deque<LineConfiguration> ColumnsConfiguration => _columnsConfiguration;

        public int ColumnLenght => _columnLenght;

        public int RowLenght => _rowLenght;

        public bool IsInitialized => isInitialized;

        public void CalculateCellInBound(Func<Vector3, bool> predicate)
        {
            for (var col = 0; col < _columnLenght; col++)
            {
                for (var row = 0; row < _rowLenght; row++)
                {
                    var gridConfiguration = _gridConfigurations[row,col];
                    var visible = predicate.Invoke(gridConfiguration.GetOrientation().Position);
                    gridConfiguration.SetActive(visible);
                }
            }
        }

        public void Clear()
        {
            for (var col = 0; col < _columnLenght; col++)
            {
                for (var row = 0; row < _rowLenght; row++)
                {
                    _gridConfigurations[row, col].Destroy();
                }
            }

            _rowConfiguration = new Deque<LineConfiguration>();
            _columnsConfiguration = new Deque<LineConfiguration>();
            _gridConfigurations = new ICellEntity[0,0];
            _columnLenght = 0;
            _rowLenght = 0;
        }
    }
}
