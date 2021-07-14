using System;
using System.Linq;
using Grid;

namespace Extensions
{
    public static class MatrixExtensions
    {
        /// <summary>
        /// Returns the row with number 'row' of this matrix as a 1D-Array.
        /// </summary>
        public static T[] GetRow<T>(this T[,] matrix, int row)
        {
            var rowLength = matrix.GetLength(1);
            var rowVector = new T[rowLength];

            for (var i = 0; i < rowLength; i++)
                rowVector[i] = matrix[row, i];

            return rowVector;
        }



        /// <summary>
        /// Sets the row with number 'row' of this 2D-matrix to the parameter 'rowVector'.
        /// </summary>
        public static void SetRow<T>(this T[,] matrix, int row, T[] rowVector)
        {
            var rowLength = matrix.GetLength(1);

            for (var i = 0; i < rowLength; i++)
                matrix[row, i] = rowVector[i];
        }



        /// <summary>
        /// Returns the column with number 'col' of this matrix as a 1D-Array.
        /// </summary>
        public static T[] GetColumn<T>(this T[,] matrix, int column)
        {
            var colLength = matrix.GetLength(0);
            var colVector = new T[colLength];

            for (var i = 0; i < colLength; i++)
                colVector[i] = matrix[i, column];

            return colVector;
        }



        /// <summary>
        /// Sets the column with number 'col' of this 2D-matrix to the parameter 'colVector'.
        /// </summary>
        public static void SetColumn<T>(this T[,] matrix, int column, T[] colVector)
        {
            var colLength = matrix.GetLength(0);

            for (var i = 0; i < colLength; i++)
                matrix[i, column] = colVector[i];
        }
        public static T[] FillDimension<T, V>(this V[,] bufferList, int dimension, Func<V[], T> onCreateInstance = null) where T : new()
        {
            var lineCount = bufferList.GetLength(dimension);
            var bufferLines = new T[lineCount];

            for (var z = 0; z < lineCount; z++)
            {
                bufferLines[z] = onCreateInstance == null ? new T() : onCreateInstance.Invoke(bufferList.GetColumn(z));
            }
            return bufferLines;
        }
        
        public static T[] FillDimension<T, V>(this V[,] bufferList, MatrixDimension dimension, Func<V[], T> onCreateInstance = null) where T : new()
        {
            return bufferList.FillDimension((int)dimension, onCreateInstance);
        }
    }

    public enum MatrixDimension : int
    {
        Row = 0,
        Column = 1
    }
}
