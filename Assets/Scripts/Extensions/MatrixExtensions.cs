using System;
using System.Collections.Generic;
using System.Linq;

namespace Extensions
{
    public static class MatrixExtensions
    {
        public static int LastDiffer<T>(this IEnumerable<T> collection)
        {
            var buffer = collection.ToArray();
            var last = buffer[0];
            var lastDiffer = 0;
            if (buffer.All(x => EqualityComparer<T>.Default.Equals(last, x))) return 0;

            for (var i = 1; i < buffer.Length; i++)
            {
                if (EqualityComparer<T>.Default.Equals(last, buffer[i]))
                {
                    lastDiffer++;
                    continue;
                }
                break;
            }
            return lastDiffer;
        }

        public static T[,] AppendRow<T>(this T[,] matrix, T[] appended)
        {
            var rowLength = matrix.GetLength((int) MatrixDimension.Row);

            if (rowLength != appended.Length)
                throw new ArgumentException("Matrix row lenght not equal appended lenght");
            var colLength = matrix.GetLength((int) MatrixDimension.Column);
            var newMatrix = new T[rowLength, colLength + 1];

            for (var row = 0; row < rowLength; row++)
            {
                for (var col = 0; col < colLength; col++) newMatrix[row, col] = matrix[row, col];
            }
            newMatrix.SetColumn(colLength, appended);
            return newMatrix;
        }

        public static T[,] PrependRow<T>(this T[,] matrix, T[] appended)
        {
            var rowLength = matrix.GetLength((int) MatrixDimension.Row);

            if (rowLength != appended.Length)
                throw new ArgumentException("Matrix row lenght not equal appended lenght");
            var colLength = matrix.GetLength((int) MatrixDimension.Column);
            var newMatrix = new T[rowLength, colLength + 1];

            for (var row = 0; row < rowLength; row++)
            {
                for (var col = 0; col < colLength; col++) newMatrix[row, col + 1] = matrix[row, col];
            }
            newMatrix.SetColumn(0, appended);
            return newMatrix;
        }

        public static T[,] AppendColumn<T>(this T[,] matrix, T[] appended)
        {
            var colLength = matrix.GetLength((int) MatrixDimension.Column);

            if (colLength != appended.Length)
                throw new ArgumentException("Matrix column lenght not equal appended lenght");
            var rowLength = matrix.GetLength((int) MatrixDimension.Row);
            var newMatrix = new T[rowLength + 1, colLength];

            for (var row = 0; row < rowLength; row++)
            {
                for (var col = 0; col < colLength; col++) newMatrix[row, col] = matrix[row, col];
            }
            newMatrix.SetRow(rowLength, appended);
            return newMatrix;
        }

        public static T[,] PrependColumn<T>(this T[,] matrix, T[] appended)
        {
            var colLength = matrix.GetLength((int) MatrixDimension.Column);

            if (colLength != appended.Length)
                throw new ArgumentException("Matrix column lenght not equal appended lenght");
            var rowLength = matrix.GetLength((int) MatrixDimension.Row);
            var newMatrix = new T[rowLength + 1, colLength];

            for (var row = 0; row < rowLength; row++)
            {
                for (var col = 0; col < colLength; col++) newMatrix[row + 1, col] = matrix[row, col];
            }
            newMatrix.SetRow(0, appended);
            return newMatrix;
        }

        /// <summary>
        ///     Returns the row with number 'row' of this matrix as a 1D-Array.
        /// </summary>
        public static T[] GetRow<T>(this T[,] matrix, int row)
        {
            var rowLength = matrix.GetLength((int) MatrixDimension.Row);
            var rowVector = new T[rowLength];
            for (var i = 0; i < rowLength; i++) rowVector[i] = matrix[i, row];
            return rowVector;
        }

        /// <summary>
        ///     Sets the row with number 'row' of this 2D-matrix to the parameter 'rowVector'.
        /// </summary>
        public static void SetColumn<T>(this T[,] matrix, int row, T[] rowVector)
        {
            var rowLength = matrix.GetLength((int) MatrixDimension.Row);
            for (var i = 0; i < rowLength; i++) matrix[i, row] = rowVector[i];
        }

        /// <summary>
        ///     Returns the column with number 'col' of this matrix as a 1D-Array.
        /// </summary>
        public static T[] GetColumn<T>(this T[,] matrix, int column)
        {
            var colLength = matrix.GetLength((int) MatrixDimension.Column);
            var colVector = new T[colLength];
            for (var i = 0; i < colLength; i++) colVector[i] = matrix[column, i];
            return colVector;
        }

        /// <summary>
        ///     Sets the column with number 'col' of this 2D-matrix to the parameter 'colVector'.
        /// </summary>
        public static void SetRow<T>(this T[,] matrix, int column, T[] colVector)
        {
            var colLength = matrix.GetLength((int) MatrixDimension.Column);
            for (var i = 0; i < colLength; i++) matrix[column, i] = colVector[i];
        }

        public static T[] FillDimension<T, TV>(this TV[,] bufferList, MatrixDimension dimension,
                                              Func<TV[], T> onCreateInstance = null) where T : new()
        {
            var lineCount = bufferList.GetLength((int) dimension);
            var bufferLines = new T[lineCount];

            switch (dimension)
            {
                case MatrixDimension.Row:
                    for (var z = 0; z < lineCount; z++)
                        bufferLines[z] = onCreateInstance == null
                                             ? new T()
                                             : onCreateInstance.Invoke(bufferList.GetColumn(z));
                    break;
                case MatrixDimension.Column:
                    for (var z = 0; z < lineCount; z++)
                        bufferLines[z] = onCreateInstance == null
                                             ? new T()
                                             : onCreateInstance.Invoke(bufferList.GetRow(z));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dimension), dimension, null);
            }
            return bufferLines;
        }
    }
}
