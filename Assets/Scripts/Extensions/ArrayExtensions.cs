using System;
using System.Linq;
using Grid;
using UnityEngine;

namespace Extensions
{
    public static class MatrixExtensions
    {
        /// <summary>
        /// Returns the row with number 'row' of this matrix as a 1D-Array.
        /// </summary>
        public static T[] GetRow<T>(this T[,] matrix, int row)
        {
            var rowLength = matrix.GetLength((int)MatrixDimension.Column);
            var rowVector = new T[rowLength];

            for (var i = 0; i < rowLength; i++)
                rowVector[i] = matrix[i, row];

            return rowVector;
        }

        /// <summary>
        /// Sets the row with number 'row' of this 2D-matrix to the parameter 'rowVector'.
        /// </summary>
        public static void SetRow<T>(this T[,] matrix, int row, T[] rowVector)
        {
            var rowLength = matrix.GetLength((int)MatrixDimension.Column);

            for (var i = 0; i < rowLength; i++)
                matrix[i, row] = rowVector[i];
        }

        /// <summary>
        /// Returns the column with number 'col' of this matrix as a 1D-Array.
        /// </summary>
        public static T[] GetColumn<T>(this T[,] matrix, int column)
        {
            var colLength = matrix.GetLength((int)MatrixDimension.Row);
            var colVector = new T[colLength];

            for (var i = 0; i < colLength; i++)
                colVector[i] = matrix[column, i];

            return colVector;
        }

        public static EntityType Negative(this EntityType entityType)
        {
            var current = (int) entityType;

            var next = current * -1;
            
            return (EntityType)next;
        }


        /// <summary>
        /// Sets the column with number 'col' of this 2D-matrix to the parameter 'colVector'.
        /// </summary>
        public static void SetColumn<T>(this T[,] matrix, int column, T[] colVector)
        {
            var colLength = matrix.GetLength((int)MatrixDimension.Row);

            for (var i = 0; i < colLength; i++)
                matrix[column, i] = colVector[i];
        }
        
        public static T[] FillDimension<T, V>(this V[,] bufferList, MatrixDimension dimension, Func<V[], T> onCreateInstance = null) where T : new()
        {
            var lineCount = bufferList.GetLength((int)dimension);
            var bufferLines = new T[lineCount];

            switch (dimension)
            {
                case MatrixDimension.Column:
                    for (var z = 0; z < lineCount; z++)
                    {
                        bufferLines[z] = onCreateInstance == null ? new T() : onCreateInstance.Invoke(bufferList.GetColumn(z));
                    }
                    break;
                case MatrixDimension.Row:
                    for (var z = 0; z < lineCount; z++)
                    {
                        bufferLines[z] = onCreateInstance == null ? new T() : onCreateInstance.Invoke(bufferList.GetRow(z));
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dimension), dimension, null);
            }
            return bufferLines;
        }
    }

    public enum MatrixDimension : int
    {
        Column = 0,
        Row = 1
    }
}
