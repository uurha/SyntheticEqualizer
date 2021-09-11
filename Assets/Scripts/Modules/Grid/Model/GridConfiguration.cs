using System;
using Extensions;
using SubModules.Cell.Interfaces;
using UnityEngine;

namespace Modules.Grid.Model
{
    public struct GridConfiguration
    {
        public LineConfiguration[] RowConfiguration { get; private set; }

        public LineConfiguration[] ColumnsConfiguration { get; private set; }

        public int ColumnLenght { get; private set; }

        public int RowLenght { get; private set; }

        public bool IsInitialized { get; private set; }

        public GridConfiguration(ICellEntity[,] cellEntities)
        {
            RowConfiguration =
                cellEntities.FillDimension(MatrixDimension.Row, entities => new LineConfiguration(entities));

            ColumnsConfiguration =
                cellEntities.FillDimension(MatrixDimension.Column, entities => new LineConfiguration(entities));
            ColumnLenght = cellEntities.GetLength((int) MatrixDimension.Column);
            RowLenght = cellEntities.GetLength((int) MatrixDimension.Row);
            IsInitialized = true;
        }

        public void CalculateCellInBound(Func<Vector3, bool> predicate)
        {
            for (var col = 0; col < ColumnLenght; col++)
            {
                for (var row = 0; row < RowLenght; row++)
                {
                    var gridConfiguration = RowConfiguration[row].GetCells()[col];
                    var visible = predicate.Invoke(gridConfiguration.GetOrientation().Position);
                    gridConfiguration.SetActive(visible);
                }
            }
        }

        public Vector3 GetLineSize(MatrixDimension dimension, int index)
        {
            if (RowConfiguration == null ||
                ColumnsConfiguration == null)
                return Vector3.zero;

            var lineSize = dimension switch
                           {
                               MatrixDimension.Row => RowConfiguration[index].GetCells().Sum(x => x.CellSize),
                               MatrixDimension.Column => ColumnsConfiguration[index].GetCells().Sum(x => x.CellSize),
                               _ => throw new ArgumentOutOfRangeException(nameof(dimension), dimension, null)
                           };
            return lineSize;
        }

        public void Clear()
        {
            for (var row = 0; row < RowLenght; row++) RowConfiguration[row].DestroyAll();
            RowConfiguration = new LineConfiguration[0];
            ColumnsConfiguration = new LineConfiguration[0];
            ColumnLenght = 0;
            RowLenght = 0;
            IsInitialized = false;
        }
    }
}
